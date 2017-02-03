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
        public ProjectModel Project
        {
            get
            {
                return (new ViewModelLocator()).Main.Project;
            }
        }

        public RelayCommand AddElementCommand { get; private set; }

        public RelayCommand<ObservableObject> OpenFileCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddElementCommand = new RelayCommand(AddElement);
            OpenFileCommand = new RelayCommand<ObservableObject>(OpenFile);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement()
        {
            //switch(para)
            //{
            //    case "FocusTree" :
            //        (new ViewModelLocator()).Main.Project.fociList.Add(
            //            new FocusGridModel("FocusTree_" + Project.fociList.Count().ToString()));
            //        RaisePropertyChanged("fociContainerList");
            //        break;
            //    case "Localisation":
            //        (new ViewModelLocator()).Main.Project.localisationList.Add(
            //            new LocalisationModel("Localisation_" + Project.localisationList.Count().ToString()));
            //        RaisePropertyChanged("localisationList");
            //        break;
            //    case "Event":
            //        (new ViewModelLocator()).Main.Project.eventList.Add(
            //            new EventTabModel("Event_" + Project.eventList.Count().ToString()));
            //        RaisePropertyChanged("eventList");
            //        break;
            //    case "Generic":
            //        (new ViewModelLocator()).Main.Project.scriptList.Add(
            //            new ScriptModel("Script" + Project.eventList.Count().ToString()));
            //        RaisePropertyChanged("scriptList");
            //        break;
            //}
        }

        private void DeleteElement(object item)
        {
            if (item is FocusGridModel)
            {
                (new ViewModelLocator()).Main.Project.fociList.Remove((FocusGridModel)item);
                RaisePropertyChanged("fociContainerList");
            }
            else if (item is LocalisationModel)
            {
                (new ViewModelLocator()).Main.Project.localisationList.Remove((LocalisationModel)item);
                RaisePropertyChanged("localisationList");
            }
            else if (item is EventTabModel)
            {
                (new ViewModelLocator()).Main.Project.eventList.Remove((EventTabModel)item);
                RaisePropertyChanged("eventList");
            }
            else if (item is ScriptModel)
            {
                (new ViewModelLocator()).Main.Project.scriptList.Remove((ScriptModel)item);
                RaisePropertyChanged("scriptList");
            }
        }

        private void OpenFile(ObservableObject item)
        {
            if (item is FocusGridModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenFocusTree"));
            }
            else if (item is LocalisationModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenLocalisation"));
            }
            else if (item is EventTabModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenEventList"));
            }
            else if (item is ScriptModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenScriptList"));
            }
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
                RaisePropertyChanged(() => Project);
                RaisePropertyChanged(() => Project.fociList);
                RaisePropertyChanged(() => Project.localisationList);
                RaisePropertyChanged(() => Project.eventList);
                RaisePropertyChanged(() => Project.scriptList);
            }
        }
    }
}