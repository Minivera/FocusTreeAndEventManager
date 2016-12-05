using FocusTreeManager.DataContract;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;

namespace FocusTreeManager.Model
{
    public class EventTabModel : ObservableObject
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        public string Filename
        {
            get
            {
                var element = Project.Instance.getSpecificEventList(ID);
                return element != null ? element.ContainerID : null;
            }
        }

        public string EventNamespace
        {
            get
            {
                return Project.Instance.getSpecificEventList(ID).EventNamespace;
            }
            set
            {
                Project.Instance.getSpecificEventList(ID).EventNamespace = value;
                RaisePropertyChanged("EventNamespace");
            }
        }

        public ObservableCollection<EventModel> EventList
        {
            get
            {
                return Project.Instance.getSpecificEventList(ID).getFocusModelList();
            }
        }

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

        public EventTabModel(Guid ID)
        {
            this.ID = ID;
            //Command
            AddEventCommand = new RelayCommand(AddEvent);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddEvent()
        {
            Event newEvent = new Event();
            newEvent.setDefaults(EventNamespace);
            Project.Instance.getSpecificEventList(ID).EventList.Add(newEvent);
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
                    Project.Instance.getSpecificEventList(ID).EventList.Remove(sender.DataContract);
                    RaisePropertyChanged(() => EventList);
                    break;
            }
        }
    }
}
