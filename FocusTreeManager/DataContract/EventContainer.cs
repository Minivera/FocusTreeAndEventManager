using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Event))]
    [KnownType(typeof(FileInfo))]
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

        [DataMember(Name = "file", Order = 4)]
        public FileInfo FileInfo { get; set; }

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

        public EventContainer(EventTabModel item)
        {
            IdentifierID = item.UniqueID;
            ContainerID = item.VisibleName;
            EventNamespace = item.EventNamespace;
            FileInfo = item.FileInfo;
            EventList = new List<Event>();
            foreach (EventModel model in item.EventList)
            {
                EventList.Add(new Event(model));
            }
        }
    }
}
