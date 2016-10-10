using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.IO;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.Parsers;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProjectViewViewModel : ViewModelBase
    {
        public ObservableObject SelectedFile { get; private set; }

        public ObservableCollection<FociGridContainer> fociContainerList
        {
            get
            {
                if ((new ViewModelLocator()).Main.isProjectExist)
                { 
                    return (new ViewModelLocator()).Main.Project.fociContainerList;
                }
                return null;
            }
        }

        public ObservableCollection<LocalisationContainer> localisationList
        {
            get
            {
                if ((new ViewModelLocator()).Main.isProjectExist)
                {
                    return (new ViewModelLocator()).Main.Project.localisationList;
                }
                return null;
            }
        }

        public RelayCommand<string> AddFileCommand { get; private set; }

        public RelayCommand<string> AddElementCommand { get; private set; }

        public RelayCommand<string> OpenFileTreeCommand { get; private set; }

        public RelayCommand<string> OpenFileLocaleCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddElementCommand = new RelayCommand<string>(AddElement);
            OpenFileTreeCommand = new RelayCommand<string>(OpenFocusTree);
            OpenFileLocaleCommand = new RelayCommand<string>(OpenLocalisation);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement(string param)
        {
            switch(param)
            {
                case "FocusTree" :
                {
                    fociContainerList.Add(new FociGridContainer("FocusTree" + fociContainerList.Count().ToString()));
                    RaisePropertyChanged("fociContainerList");
                    break;
                }
                case "Localisation":
                {
                    localisationList.Add(new LocalisationContainer("Localisation" + localisationList.Count().ToString()));
                    RaisePropertyChanged("localisationList");
                    break;
                }
            }
        }

        private void DeleteElement(object item)
        {
            if (item is FociGridContainer)
            {
                fociContainerList.Remove((FociGridContainer)item);
                RaisePropertyChanged("fociContainerList");
            }
            else if (item is LocalisationContainer)
            {
                localisationList.Remove((LocalisationContainer)item);
                RaisePropertyChanged("localisationList");
            }
        }

        private void AddFile(string param)
        {
            var dialog = new CommonOpenFileDialog();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            dialog.Title = resourceLocalization["Add_Game_File"] as string;
            dialog.InitialDirectory = Configurator.getGamePath();
            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = "C:";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                switch (param)
                {
                    case "FocusTree":
                        {
                            Script script = new Script();
                            script.Analyse(File.ReadAllText(dialog.FileName));
                            fociContainerList.Add(FocusTreeParser.CreateTreeFromScript(dialog.FileName, script));
                            RaisePropertyChanged("fociContainerList");
                            break;
                        }
                    case "Localisation":
                        {
                            localisationList.Add(LocalisationParser.CreateLocaleFromFile(dialog.FileName));
                            RaisePropertyChanged("fociContainerList");
                            break;
                        }
                }
            }
        }

        private void OpenFocusTree(string param)
        {
            SelectedFile = fociContainerList.SingleOrDefault((f) => f.ContainerID == param);
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "OpenFocusTree"));
        }

        private void OpenLocalisation(string param)
        {
            SelectedFile = localisationList.SingleOrDefault((f) => f.ContainerID == param);
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "OpenLocalisation"));
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Target != null && msg.Target != this)
            {
                //Message not itended for here
                return;
            }
            if (msg.Notification == "SendDeleteItemSignal")
            {
                DeleteElement(msg.Sender);
            }
            if (msg.Notification == "RefreshProjectViewer")
            {
                RaisePropertyChanged("fociContainerList");
                RaisePropertyChanged("localisationList");
            }
        }
    }
}