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
using FocusTreeManager.DataContract;
using System.Collections.Generic;

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
        public ObservableCollection<FociGridContainer> fociContainerList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    ObservableCollection<FociGridContainer> list = 
                        new ObservableCollection<FociGridContainer>();
                    foreach (var item in Project.Instance.fociContainerList)
                    {
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
        }

        public ObservableCollection<LocalisationContainer> localisationList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    ObservableCollection<LocalisationContainer> list =
                        new ObservableCollection<LocalisationContainer>();
                    foreach (var item in Project.Instance.localisationList)
                    {
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
        }

        public ObservableCollection<EventContainer> eventList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    ObservableCollection<EventContainer> list =
                        new ObservableCollection<EventContainer>();
                    foreach (var item in Project.Instance.eventList)
                    {
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
        }

        public ObservableCollection<ScriptContainer> scriptList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    ObservableCollection<ScriptContainer> list =
                        new ObservableCollection<ScriptContainer>();
                    foreach (var item in Project.Instance.scriptList)
                    {
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
        }

        public RelayCommand<string> AddFileCommand { get; private set; }

        public RelayCommand<string> AddElementCommand { get; private set; }

        public RelayCommand<Guid> OpenFileTreeCommand { get; private set; }

        public RelayCommand<Guid> OpenFileLocaleCommand { get; private set; }

        public RelayCommand<Guid> OpenFileEventCommand { get; private set; }

        public RelayCommand<Guid> OpenFileGenericCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddElementCommand = new RelayCommand<string>(AddElement);
            OpenFileTreeCommand = new RelayCommand<Guid>(OpenFocusTree);
            OpenFileLocaleCommand = new RelayCommand<Guid>(OpenLocalisation);
            OpenFileEventCommand = new RelayCommand<Guid>(OpenEvent);
            OpenFileGenericCommand = new RelayCommand<Guid>(OpenScript);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement(string param)
        {
            switch(param)
            {
                case "FocusTree" :
                    Project.Instance.fociContainerList.Add(new FociGridContainer("FocusTree_" + 
                        fociContainerList.Count().ToString()));
                    RaisePropertyChanged("fociContainerList");
                    break;
                case "Localisation":
                    Project.Instance.localisationList.Add(new LocalisationContainer("Localisation_" + 
                        localisationList.Count().ToString()));
                    RaisePropertyChanged("localisationList");
                    break;
                case "Event":
                    Project.Instance.eventList.Add(new EventContainer("Event_" + eventList.Count().ToString()));
                    RaisePropertyChanged("eventList");
                    break;
                case "Generic":
                    Project.Instance.scriptList.Add(new ScriptContainer("Script" + eventList.Count().ToString()));
                    RaisePropertyChanged("scriptList");
                    break;
            }
        }

        private void DeleteElement(object item)
        {
            if (item is FociGridContainer)
            {
                Project.Instance.fociContainerList.Remove((FociGridContainer)item);
                RaisePropertyChanged("fociContainerList");
            }
            else if (item is LocalisationContainer)
            {
                Project.Instance.localisationList.Remove((LocalisationContainer)item);
                RaisePropertyChanged("localisationList");
            }
            else if (item is EventContainer)
            {
                Project.Instance.eventList.Remove((EventContainer)item);
                RaisePropertyChanged("eventList");
            }
            else if (item is ScriptContainer)
            {
                Project.Instance.scriptList.Remove((ScriptContainer)item);
                RaisePropertyChanged("scriptList");
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
                try
                {
                    switch(param)
                    {
                        case "FocusTree":
                            Script script = new Script();
                            script.Analyse(File.ReadAllText(dialog.FileName));
                            Project.Instance.fociContainerList.Add(FocusTreeParser.CreateTreeFromScript
                                (dialog.FileName, script));
                            RaisePropertyChanged("fociContainerList");
                            break;
                        case "Localisation":
                            Project.Instance.localisationList.Add(LocalisationParser.CreateLocaleFromFile
                                (dialog.FileName));
                            RaisePropertyChanged("localisationList");
                            break;
                        case "Event":
                            Script scriptEvent = new Script();
                            scriptEvent.Analyse(File.ReadAllText(dialog.FileName));
                            Project.Instance.eventList.Add(EventParser.CreateEventFromScript
                                (dialog.FileName, scriptEvent));
                            RaisePropertyChanged("eventList");
                            break;
                        case "Generic":
                            Project.Instance.scriptList.Add(ScriptParser.CreateScriptFromFile
                                (dialog.FileName));
                            RaisePropertyChanged("scriptList");
                            break;
                    }
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Impossible to load " + dialog.FileName + 
                        ", file is not valid. Please check your syntax and try again.");
                }
            }
        }

        private void OpenFocusTree(Guid param)
        {
            FociGridContainer SelectedFile = fociContainerList.SingleOrDefault((f) => f.IdentifierID == param);
            Messenger.Default.Send(new NotificationMessage(SelectedFile, (new ViewModelLocator()).Main, "OpenFocusTree"));
        }

        private void OpenLocalisation(Guid param)
        {
            LocalisationContainer SelectedFile = localisationList.SingleOrDefault((f) => f.IdentifierID == param);
            Messenger.Default.Send(new NotificationMessage(SelectedFile, (new ViewModelLocator()).Main, "OpenLocalisation"));
        }

        private void OpenEvent(Guid param)
        {
            EventContainer SelectedFile = eventList.SingleOrDefault((f) => f.IdentifierID == param);
            Messenger.Default.Send(new NotificationMessage(SelectedFile, (new ViewModelLocator()).Main, "OpenEventList"));
        }

        private void OpenScript(Guid param)
        {
            ScriptContainer SelectedFile = scriptList.SingleOrDefault((f) => f.IdentifierID == param);
            Messenger.Default.Send(new NotificationMessage(SelectedFile, (new ViewModelLocator()).Main, "OpenScriptList"));
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
                RaisePropertyChanged("eventList");
                RaisePropertyChanged("scriptList");
            }
        }
    }
}