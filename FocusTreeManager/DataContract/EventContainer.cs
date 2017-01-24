using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
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

        public EventContainer()
        {
            IdentifierID = Guid.NewGuid();
            EventList = new List<Event>();
        }

        public EventContainer(string filename)
        {
            ContainerID = filename;
            EventList = new List<Event>();
            IdentifierID = Guid.NewGuid();
        }

        public EventContainer(Containers.LegacySerialization.EventContainer legacyItem)
        {
            ContainerID = legacyItem.ContainerID;
            EventNamespace = legacyItem.EventNamespace;
            EventList = Event.PopulateFromLegacy(legacyItem.EventList.ToList());
            IdentifierID = legacyItem.IdentifierID;
        }

        public EventContainer(EventTabModel item)
        {
            IdentifierID = item.UniqueID;
            ContainerID = item.Filename;
            EventNamespace = item.EventNamespace;
            EventList = new List<Event>();
            foreach (EventModel model in item.EventList)
            {
                EventList.Add(new Event(model));
            }
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
    }
}
