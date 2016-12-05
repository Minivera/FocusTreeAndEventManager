using FocusTreeManager.CodeStructures;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System;
using FocusTreeManager.Model;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(EventType))]
    [KnownType(typeof(Script))]
    [KnownType(typeof(EventOption))]
    [KnownType(typeof(EventDescription))]
    [DataContract(Name = "event")]
    public class Event
    {
        const string IMAGE_PATH = "/FocusTreeManager;component/GFX/Events/";

        [DataContract(Name = "event_type")]
        public enum EventType
        {
            [EnumMember]
            news_event,
            [EnumMember]
            country_event
        }

        [DataMember(Name = "id", Order = 0)]
        public string Id { get; set; }

        [DataMember(Name = "type", Order = 1)]
        public EventType Type { get; set; }

        [DataMember(Name = "picture", Order = 1)]
        public string Picture { get; set; }

        public string ImagePath
        {
            get
            {
                return IMAGE_PATH + Picture + ".png";
            }
        }

        [DataMember(Name = "is_major", Order = 2)]
        public bool IsMajor { get; set; }

        [DataMember(Name = "is_triggered", Order = 3)]
        public bool IsTriggeredOnly { get; set; }

        [DataMember(Name = "is_hidden", Order = 4)]
        public bool IsHidden { get; set; }

        [DataMember(Name = "is_once", Order = 5)]
        public bool IsFiredOnce { get; set; }

        [DataMember(Name = "script", Order = 6)]
        public Script InternalScript { get; set; }

        [DataMember(Name = "descriptions", Order = 7)]
        public List<EventDescription> Descriptions { get; set;}

        [DataMember(Name = "options", Order = 8)]
        public List<EventOption> Options { get; set; }

        public Event()
        {
            Descriptions = new List<EventDescription>();
            Options = new List<EventOption>();
        }

        public Event(Model.LegacySerialization.Event legacyItem)
        {
            Id = legacyItem.Id;
            Type = legacyItem.Type == Model.LegacySerialization.Event.EventType.country_event ? 
                EventType.country_event : EventType.news_event;
            Picture = legacyItem.Picture;
            IsFiredOnce = legacyItem.IsFiredOnce;
            IsHidden = legacyItem.IsHidden;
            IsMajor = legacyItem.IsMajor;
            IsTriggeredOnly = legacyItem.IsTriggeredOnly;
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
            Descriptions = EventDescription.PopulateFromLegacy(legacyItem.Descriptions);
            Options = EventOption.PopulateFromLegacy(legacyItem.Options);
        }

        internal static List<Event> PopulateFromLegacy(
            List<Model.LegacySerialization.Event> eventList)
        {
            List<Event> list = new List<Event>();
            foreach (Model.LegacySerialization.Event legacyItem in eventList)
            {
                list.Add(new Event(legacyItem));
            }
            return list;
        }

        public void setDefaults(string EventNamespace)
        {
            if (string.IsNullOrEmpty(EventNamespace))
            {
                EventNamespace = "namespace";
            }
            Id = EventNamespace + ".0";
            Picture = "event_test";
            Type = EventType.country_event;
            InternalScript = new Script();
            EventDescription newDesc = new EventDescription();
            newDesc.setDefaults();
            Descriptions.Add(newDesc);
            EventOption newOptions = new EventOption();
            newOptions.setDefaults();
            Options.Add(newOptions);
        }

        internal List<EventDescriptionModel> getDescriptionModels()
        {
            List<EventDescriptionModel> list = new List<EventDescriptionModel>();
            foreach (EventDescription item in Descriptions)
            {
                list.Add(new EventDescriptionModel(item));
            }
            return list;
        }

        internal List<EventOptionModel> getOptionModels()
        {
            List<EventOptionModel> list = new List<EventOptionModel>();
            foreach (EventOption item in Options)
            {
                list.Add(new EventOptionModel(item));
            }
            return list;
        }
    }
}
