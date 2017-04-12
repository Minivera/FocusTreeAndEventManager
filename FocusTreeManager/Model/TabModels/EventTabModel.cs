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

        public RelayCommand AddEventCommand { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public RelayCommand CopyElementCommand { get; private set; }

        public EventTabModel(string Filename)
        {
            visbleName = Filename;
            UniqueID = Guid.NewGuid();
            EventList = new ObservableCollection<EventModel>();
            EventList.CollectionChanged += EventList_CollectionChanged;
            SetupCommons();
        }

        public EventTabModel(EventContainer container)
        {
            UniqueID = container.IdentifierID;
            visbleName = container.ContainerID;
            eventNamespace = container.EventNamespace;
            fileInfo = container.FileInfo;
            EventList = new ObservableCollection<EventModel>();
            foreach (Event item in container.EventList)
            {
                EventList.Add(new EventModel(item));
            }
            EventList.CollectionChanged += EventList_CollectionChanged;
            SetupCommons();
        }

        public EventTabModel(EventTabModel model)
        {
            UniqueID = Guid.NewGuid();
            eventNamespace = model.EventNamespace;
            fileInfo = model.FileInfo;
            EventList = new ObservableCollection<EventModel>();
            foreach (EventModel item in model.EventList)
            {
                EventList.Add(new EventModel(item));
            }
            EventList.CollectionChanged += EventList_CollectionChanged;
            SetupCommons();
        }

        private void SetupCommons()
        {
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            CopyElementCommand = new RelayCommand(SendCopySignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddEvent()
        {
            UndoService.Current[GetUndoRoot()]
                .BeginChangeSetBatch("AddEvent", false);
            EventModel newEvent = new EventModel();
            newEvent.setDefaults(EventNamespace);
            EventList.Add(newEvent);
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
            RaisePropertyChanged(() => EventList);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            //If this is a dead tab waiting to be destroyed
            if (VisibleName == null) return;
            if (msg.Notification == "DeleteEvent")
            {
                EventModel sender = msg.Sender as EventModel;
                EventList.Remove(sender);
                RaisePropertyChanged(() => EventList);
            }
            if (msg.Target == this)
            {
                //Resend to the tutorial View model if this was the target
                Messenger.Default.Send(new NotificationMessage(msg.Sender,
                new ViewModelLocator().Tutorial, msg.Notification));
            }
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendEditItemSignal"));
        }

        private void SendCopySignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendCopyItemSignal"));
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
                new ViewModelLocator().CodeComparator.DiffModel = model;
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
