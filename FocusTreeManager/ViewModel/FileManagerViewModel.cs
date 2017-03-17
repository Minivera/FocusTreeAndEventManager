using FocusTreeManager.CodeStructures;
using FocusTreeManager.CodeStructures.CodeExceptions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.ViewModel
{
    public class FileManagerViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator coordinator;

        private ObservableObject file;

        public ObservableObject File
        {
            get
            {
                return file;
            }
            set
            {
                if (value == file)
                {
                    return;
                }
                file = value;
                RaisePropertyChanged(() => File);
            }
        }

        public RelayCommand AddGameFileCommand { get; set; }

        public RelayCommand<object> FileTypeCommand { get; set; }

        public RelayCommand AcceptCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public FileManagerViewModel()
        {
            coordinator = DialogCoordinator.Instance;
            AddGameFileCommand = new RelayCommand(AddGameFile);
            FileTypeCommand = new RelayCommand<object>(SetFileType);
            AcceptCommand = new RelayCommand(Accept);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void AddGameFile()
        {
            File = null;
            try
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary
                {
                    Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
                };
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    Title = resourceLocalization["Add_Game_File"] as string,
                    InitialDirectory = Configurator.getGamePath(),
                    AddToMostRecentlyUsedList = false,
                    AllowNonFileSystemItems = false,
                    DefaultDirectory = "C:",
                    EnsureFileExists = true,
                    EnsurePathExists = true,
                    EnsureReadOnly = false,
                    EnsureValidNames = true
                };
                dialog.Filters.Add(new CommonFileDialogFilter("Scripts", "*.txt"));
                dialog.Filters.Add(new CommonFileDialogFilter("Localization", "*.yml"));
                dialog.Filters.Add(new CommonFileDialogFilter("All", "*.*"));
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    //check if the file is a .yaml file
                    if (Path.GetExtension(dialog.FileName) == ".yml")
                    {
                        File = Parsers.LocalisationParser.CreateLocaleFromFile(dialog.FileName);
                        ((LocalisationModel)File).FileInfo = new DataContract.FileInfo
                        {
                            Filename = dialog.FileName,
                            LastModifiedDate = System.IO.File.GetLastWriteTime(dialog.FileName)
                        };
                    }
                    else
                    { 
                        string contents = System.IO.File.ReadAllText(dialog.FileName);
                        Script script = new Script();
                        script.Analyse(contents);
                        try
                        {
                            //If a focus tree
                            if (script.FindAssignation("focus_tree") != null)
                            {
                                File = Parsers.FocusTreeParser.CreateTreeFromScript(dialog.FileName, 
                                    script);
                                ((FocusGridModel)File).FileInfo = new DataContract.FileInfo
                                {
                                    Filename = dialog.FileName,
                                    LastModifiedDate = System.IO.File.GetLastWriteTime(dialog.FileName)
                                };
                            }
                            //If an event file
                            else if (script.FindAssignation("country_event") != null ||
                                     script.FindAssignation("news_event") != null)
                            {
                                File = Parsers.EventParser.CreateEventFromScript(dialog.FileName, 
                                    script);
                                ((EventTabModel)File).FileInfo = new DataContract.FileInfo
                                {
                                    Filename = dialog.FileName,
                                    LastModifiedDate = System.IO.File.GetLastWriteTime(dialog.FileName)
                                };
                            }
                            //If an generic file
                            else
                            {
                                File = Parsers.ScriptParser.CreateScriptFromFile(dialog.FileName);
                                ((ScriptModel)File).FileInfo = new DataContract.FileInfo
                                {
                                    Filename = dialog.FileName,
                                    LastModifiedDate = System.IO.File.GetLastWriteTime(dialog.FileName)
                                };
                            }
                        }
                        catch (Exception)
                        {
                            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), 
                                UriKind.Relative);
                            string Title = resourceLocalization["Application_Error"] as string;
                            string Message = resourceLocalization["Application_Script_Fallback"] 
                                as string;
                            coordinator.ShowMessageAsync(this, Title, Message);
                            //If it crashed, it is possible it was a generic file
                            File = Parsers.ScriptParser.CreateScriptFromFile(dialog.FileName);
                            ((ScriptModel)File).VisibleName =
                                Path.GetFileNameWithoutExtension(dialog.FileName);
                        }
                    }
                }
            }
            catch (SyntaxException e)
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary
                {
                    Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
                };
                string Title = resourceLocalization["Application_Error"] as string;
                string Message = resourceLocalization["Application_Error_Script"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
                ErrorLogger.Instance.AddLogLine(e.Message);
            }
            catch (Exception)
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary
                {
                    Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
                };
                string Title = resourceLocalization["Application_Error"] as string;
                string Message = resourceLocalization["Application_Error_Script"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
            }
            Activate();
        }

        public void SetFileType(object param)
        {
            File = null;
            ListViewItem item = param as ListViewItem;
            if (item == null)
            {
                return;
            }
            switch (item.Tag as string)
            {
                case "FocusTreeItem":
                    File = new FocusGridModel("New file");
                    break;
                case "LocalisationItem":
                    File = new LocalisationModel("New file");
                    break;
                case "EventItem":
                    File = new EventTabModel("New file");
                    break;
                case "GenericItem":
                    File = new ScriptModel("New file");
                    break;
            }
        }

        public void Accept()
        {
            Close();
        }

        public void Cancel()
        {
            File = null;
            Close();
        }

        private void Close()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }

        private void Activate()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Activate();
                }
            }
        }
    }
}