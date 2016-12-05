using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Event))]
    [DataContract(Name = "event_container")]
    public class EventContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "namespace", Order = 2)]
        public string EventNamespace { get; set; }

        [DataMember(Name = "events", Order = 3)]
        public List<Event> EventList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public EventContainer()
        {
            IdentifierID = Guid.NewGuid();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        public EventContainer(string filename)
        {
            ContainerID = filename;
            EventList = new List<Event>();
            IdentifierID = Guid.NewGuid();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        public EventContainer(Containers.LegacySerialization.EventContainer legacyItem)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            ContainerID = legacyItem.ContainerID;
            EventNamespace = legacyItem.EventNamespace;
            EventList = Event.PopulateFromLegacy(legacyItem.EventList.ToList());
            IdentifierID = legacyItem.IdentifierID;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        internal static List<EventContainer> PopulateFromLegacy(
            List<Containers.LegacySerialization.EventContainer> eventList)
        {
            List<EventContainer> list = new List<EventContainer>();
            foreach (Containers.LegacySerialization.EventContainer legacyItem in eventList)
            {
                list.Add(new EventContainer(legacyItem));
            }
            return list;
        }
        
        public ObservableCollection<EventModel> getFocusModelList()
        {
            ObservableCollection<EventModel> list = new ObservableCollection<EventModel>();
            foreach (Event item in EventList)
            {
                list.Add(new EventModel(item));
            }
            return list;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
    }
}
