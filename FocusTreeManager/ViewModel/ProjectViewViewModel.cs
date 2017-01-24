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
        public ObservableCollection<FocusGridModel> FociList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    return (new ViewModelLocator()).Main.Project.fociList;
                }
                return null;
            }
        }

        public ObservableCollection<LocalisationModel> localisationList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    return (new ViewModelLocator()).Main.Project.localisationList;
                }
                return null;
            }
        }

        public ObservableCollection<EventTabModel> eventList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    return (new ViewModelLocator()).Main.Project.eventList;
                }
                return null;
            }
        }

        public ObservableCollection<ScriptModel> scriptList
        {
            get
            {
                if ((new ViewModelLocator()).Main.IsProjectExist)
                {
                    return (new ViewModelLocator()).Main.Project.scriptList;
                }
                return null;
            }
        }

        public RelayCommand<string> AddFileCommand { get; private set; }

        public RelayCommand<string> AddElementCommand { get; private set; }

        public RelayCommand<FocusGridModel> OpenFileTreeCommand { get; private set; }

        public RelayCommand<LocalisationModel> OpenFileLocaleCommand { get; private set; }

        public RelayCommand<EventTabModel> OpenFileEventCommand { get; private set; }

        public RelayCommand<ScriptModel> OpenFileGenericCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddElementCommand = new RelayCommand<string>(AddElement);
            OpenFileTreeCommand = new RelayCommand<FocusGridModel>(OpenFocusTree);
            OpenFileLocaleCommand = new RelayCommand<LocalisationModel>(OpenLocalisation);
            OpenFileEventCommand = new RelayCommand<EventTabModel>(OpenEvent);
            OpenFileGenericCommand = new RelayCommand<ScriptModel>(OpenScript);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement(string param)
        {
            switch(param)
            {
                case "FocusTree" :
                    (new ViewModelLocator()).Main.Project.fociList.Add(
                        new FocusGridModel("FocusTree_" + FociList.Count().ToString()));
                    RaisePropertyChanged("fociContainerList");
                    break;
                case "Localisation":
                    (new ViewModelLocator()).Main.Project.localisationList.Add(
                        new LocalisationModel("Localisation_" + localisationList.Count().ToString()));
                    RaisePropertyChanged("localisationList");
                    break;
                case "Event":
                    (new ViewModelLocator()).Main.Project.eventList.Add(
                        new EventTabModel("Event_" + eventList.Count().ToString()));
                    RaisePropertyChanged("eventList");
                    break;
                case "Generic":
                    (new ViewModelLocator()).Main.Project.scriptList.Add(
                        new ScriptModel("Script" + eventList.Count().ToString()));
                    RaisePropertyChanged("scriptList");
                    break;
            }
        }

        private void DeleteElement(object item)
        {
            if (item is FociGridContainer)
            {
                (new ViewModelLocator()).Main.Project.fociList.Remove((FocusGridModel)item);
                RaisePropertyChanged("fociContainerList");
            }
            else if (item is LocalisationContainer)
            {
                (new ViewModelLocator()).Main.Project.localisationList.Remove((LocalisationModel)item);
                RaisePropertyChanged("localisationList");
            }
            else if (item is EventContainer)
            {
                (new ViewModelLocator()).Main.Project.eventList.Remove((EventTabModel)item);
                RaisePropertyChanged("eventList");
            }
            else if (item is ScriptContainer)
            {
                (new ViewModelLocator()).Main.Project.scriptList.Remove((ScriptModel)item);
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
                            (new ViewModelLocator()).Main.Project.fociList.Add(
                                FocusTreeParser.CreateTreeFromScript
                                (dialog.FileName, script));
                            RaisePropertyChanged("fociContainerList");
                            break;
                        case "Localisation":
                            (new ViewModelLocator()).Main.Project.localisationList.Add(
                                LocalisationParser.CreateLocaleFromFile
                                (dialog.FileName));
                            RaisePropertyChanged("localisationList");
                            break;
                        case "Event":
                            Script scriptEvent = new Script();
                            scriptEvent.Analyse(File.ReadAllText(dialog.FileName));
                            (new ViewModelLocator()).Main.Project.eventList.Add(
                                EventParser.CreateEventFromScript
                                (dialog.FileName, scriptEvent));
                            RaisePropertyChanged("eventList");
                            break;
                        case "Generic":
                            (new ViewModelLocator()).Main.Project.scriptList.Add(
                                ScriptParser.CreateScriptFromFile
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

        private void OpenFocusTree(FocusGridModel param)
        {
            Messenger.Default.Send(new NotificationMessage(
                param, (new ViewModelLocator()).Main, "OpenFocusTree"));
        }

        private void OpenLocalisation(LocalisationModel param)
        {
            Messenger.Default.Send(new NotificationMessage(
                param, (new ViewModelLocator()).Main, "OpenLocalisation"));
        }

        private void OpenEvent(EventTabModel param)
        {
            Messenger.Default.Send(new NotificationMessage(
                param, (new ViewModelLocator()).Main, "OpenEventList"));
        }

        private void OpenScript(ScriptModel param)
        {
            Messenger.Default.Send(new NotificationMessage(
                param, (new ViewModelLocator()).Main, "OpenScriptList"));
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
                RaisePropertyChanged(() => FociList);
                RaisePropertyChanged(() => localisationList);
                RaisePropertyChanged(() => eventList);
                RaisePropertyChanged(() => scriptList);
            }
        }
    }
}