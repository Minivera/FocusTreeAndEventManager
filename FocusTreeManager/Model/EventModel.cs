using FocusTreeManager.Containers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    public class EventModel : ObservableObject
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
                return (new ViewModelLocator()).Main.Project.getSpecificEventList(ID).ContainerID;
            }
        }

        public string EventNamespace
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificEventList(ID).EventNamespace;
            }
            set
            {
                (new ViewModelLocator()).Main.Project.getSpecificEventList(ID).EventNamespace = value;
                RaisePropertyChanged("EventNamespace");
            }
        }

        public ObservableCollection<Event> EventList
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificEventList(ID).EventList;
            }
        }

        private Event selectedNode = null;

        public Event SelectedNode
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

        public EventModel(Guid ID)
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
            EventList.Add(newEvent);
            RaisePropertyChanged(() => EventList);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
            if (!this.isShown || this.Filename == null)
            {
                //is not shown, do not manage
                return;
            }
            switch (msg.Notification)
            {
                case "DeleteEvent":
                    Event sender = msg.Sender as Event;
                    EventList.Remove(sender);
                    RaisePropertyChanged(() => EventList);
                    break;
            }
        }
    }
}
