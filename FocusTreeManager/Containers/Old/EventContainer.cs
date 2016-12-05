using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using FocusTreeManager.Containers.LegacySerialization;

namespace FocusTreeManager.Containers
{
    [KnownType(typeof(Event))]
    [DataContract(Name = "event_container")]
    public class EventContainer : ObservableObject
    {
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "id", Order = 0)]
        private string containerID;

        public string ContainerID
        {
            get
            {
                return containerID;
            }
            set
            {
                Set<string>(() => this.ContainerID, ref this.containerID, value);
                Messenger.Default.Send(new NotificationMessage("ContainerRenamed"));
            }
        }

        [DataMember(Name = "namespace", Order = 1)]
        private string eventNamespace;

        public string EventNamespace
        {
            get
            {
                return eventNamespace;
            }
            set
            {
                Set<string>(() => this.EventNamespace, ref this.eventNamespace, value);
            }
        }

        [DataMember(Name = "events", Order = 2)]
        public ObservableCollection<Event> EventList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public EventContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public EventContainer(string filename)
        {
            ContainerID = filename;
            EventList = new ObservableCollection<Event>();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public EventContainer(LegacySerialization.EventContainer legacyItem)
        {
            ContainerID = legacyItem.ContainerID;
            EventNamespace = legacyItem.EventNamespace;
            EventList = Event.PopulateFromLegacy(legacyItem.EventList);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
        }

        internal static ObservableCollection<EventContainer> PopulateFromLegacy(
            ObservableCollection<LegacySerialization.EventContainer> eventList)
        {
            ObservableCollection<EventContainer> list = new ObservableCollection<EventContainer>();
            foreach (LegacySerialization.EventContainer legacyItem in eventList)
            {
                list.Add(new EventContainer(legacyItem));
            }
            return list;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                "SendDeleteItemSignal"));
        }
    }
}
