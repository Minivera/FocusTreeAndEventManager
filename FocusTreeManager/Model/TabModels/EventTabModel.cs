using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FocusTreeManager.DataContract;
using FocusTreeManager.Parsers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using MonitoredUndo;

namespace FocusTreeManager.Model.TabModels
{
    public class EventTabModel : ObservableObject, ISupportsUndo
    {
        public Guid UniqueID { get; }

        private string visbleName;

        public string VisibleName
        {
            get
            {
                return visbleName;
            }
            set
            {
                if (value == visbleName)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "VisibleName", visbleName, value, "VisibleName Changed");
                visbleName = value;
                RaisePropertyChanged(() => VisibleName);
            }
        }

        private string eventNamespace;

        public string EventNamespace
        {
            get
            {
                return eventNamespace;
            }
            set
            {
                if (value == eventNamespace)
                {
                    return;
                }
                eventNamespace = value;
                RaisePropertyChanged(() => EventNamespace);
            }
        }

        private DataContract.FileInfo fileInfo;

        public DataContract.FileInfo FileInfo
        {
            get
            {
                return fileInfo;
            }
            set
            {
                if (value == fileInfo)
                {
                    return;
                }
                fileInfo = value;
                RaisePropertyChanged(() => FileInfo);
            }
        }

        public ObservableCollection<EventModel> EventList { get; set; }

        private EventModel selectedNode;

        public EventModel SelectedNode
        {
            get
            {
                return selectedNode;
            }
            set
            {
                if (value != selectedNode)
                {
                    selectedNode = value;
                    RaisePropertyChanged(() => SelectedNode);
                }
            }
        }

        public bool isShown { get; set; }

        public RelayCommand AddEventCommand { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public EventTabModel(string Filename)
        {
            visbleName = Filename;
            UniqueID = Guid.NewGuid();
            EventList = new ObservableCollection<EventModel>();
            EventList.CollectionChanged += EventList_CollectionChanged;
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public EventTabModel(EventContainer container)
        {
            UniqueID = container.IdentifierID;
            visbleName = container.ContainerID;
            eventNamespace = container.EventNamespace;
            FileInfo = container.FileInfo;
            EventList = new ObservableCollection<EventModel>();
            foreach (Event item in container.EventList)
            {
                EventList.Add(new EventModel(item));
            }
            EventList.CollectionChanged += EventList_CollectionChanged;
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddEvent()
        {
            EventModel newEvent = new EventModel();
            newEvent.setDefaults(EventNamespace);
            EventList.Add(newEvent);
            RaisePropertyChanged(() => EventList);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (VisibleName == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => VisibleName);
            }
            if (!isShown)
            {
                //is not shown, do not manage
                return;
            }
            if (msg.Notification == "DeleteEvent")
            {
                EventModel sender = msg.Sender as EventModel;
                EventList.Remove(sender);
                RaisePropertyChanged(() => EventList);
            }
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendEditItemSignal"));
        }

        public async void CheckForChanges()
        {
            DataContract.FileInfo info = FileInfo;
            //check the fileinfo data
            if (info == null) return;
            //If the file exists
            if (!File.Exists(info.Filename)) return;
            //If the file was modified after the last modification date
            if (File.GetLastWriteTime(info.Filename) <= info.LastModifiedDate) return;
            //Then we can show a message
            MessageDialogResult Result = await (new ViewModelLocator())
                .Main.ShowFileChangedDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                string oldText = EventParser.ParseEventForCompare(this);
                string newText = EventParser.ParseEventScriptForCompare(info.Filename);
                SideBySideDiffModel model = new SideBySideDiffBuilder(
                    new Differ()).BuildDiffModel(oldText, newText);
                (new ViewModelLocator()).CodeComparator.DiffModel = model;
                new CompareCode().ShowDialog();
            }
        }

        #region Undo/Redo

        private void EventList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "EventList",
                EventList, e, "EventList Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
