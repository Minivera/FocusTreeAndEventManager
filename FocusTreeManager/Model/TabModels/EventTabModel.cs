using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FocusTreeManager.Model
{
    public class EventTabModel : ObservableObject, ISupportsUndo
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (value == filename)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Filename", filename, value, "Filename Changed");
                filename = value;
                RaisePropertyChanged(() => Filename);
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
                DefaultChangeFactory.Current.OnChanging(this,
                         "EventNamespace", eventNamespace, value, "EventNamespace Changed");
                eventNamespace = value;
                RaisePropertyChanged(() => EventNamespace);
            }
        }

        public ObservableCollection<EventModel> EventList { get; set; }

        private EventModel selectedNode = null;

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
                    RaisePropertyChanged("SelectedVNode");
                }
            }
        }

        public bool isShown { get; set; }

        public RelayCommand AddEventCommand { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public EventTabModel(string Filename)
        {
            this.filename = Filename;
            this.ID = Guid.NewGuid();
            EventList = new ObservableCollection<EventModel>();
            EventList.CollectionChanged += EventList_CollectionChanged;
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public EventTabModel(EventContainer container)
        {
            this.ID = container.IdentifierID;
            this.filename = container.ContainerID;
            this.eventNamespace = container.EventNamespace;
            EventList = new ObservableCollection<EventModel>();
            foreach (Event item in container.EventList)
            {
                EventList.Add(new EventModel(item));
            }
            EventList.CollectionChanged += EventList_CollectionChanged;
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
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
            if (this.Filename == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
            if (!this.isShown)
            {
                //is not shown, do not manage
                return;
            }
            switch (msg.Notification)
            {
                case "DeleteEvent":
                    EventModel sender = msg.Sender as EventModel;
                    EventList.Remove(sender);
                    RaisePropertyChanged(() => EventList);
                    break;
            }
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
        #region Undo/Redo

        void EventList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "EventList", 
                this.EventList, e, "EventList Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
