using FocusTreeManager.CodeStructures;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        [DataMember(Name = "picture", Order = 2)]
        public string Picture { get; set; }

        [DataMember(Name = "is_major", Order = 3)]
        public bool IsMajor { get; set; }

        [DataMember(Name = "is_triggered", Order = 4)]
        public bool IsTriggeredOnly { get; set; }

        [DataMember(Name = "is_hidden", Order = 5)]
        public bool IsHidden { get; set; }

        [DataMember(Name = "is_once", Order = 6)]
        public bool IsFiredOnce { get; set; }

        [DataMember(Name = "script", Order = 7)]
        public Script InternalScript { get; set; }

        [DataMember(Name = "descriptions", Order = 8)]
        public List<EventDescription> Descriptions { get; set;}

        [DataMember(Name = "options", Order = 9)]
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

        public Event(EventModel model)
        {
            Id = model.Id;
            Type = model.Type;
            Picture = model.Picture;
            IsFiredOnce = model.IsFiredOnce;
            IsHidden = model.IsHidden;
            IsMajor = model.IsMajor;
            IsTriggeredOnly = model.IsTriggeredOnly;
            InternalScript = model.InternalScript;
            Descriptions = new List<EventDescription>();
            foreach (EventDescriptionModel desc in model.Descriptions)
            {
                Descriptions.Add(new EventDescription()
                {
                    InternalScript = desc.InternalScript
                });
            }
            Options = new List<EventOption>();
            foreach (EventOptionModel option in model.Options)
            {
                Options.Add(new EventOption()
                {
                    Name = option.Name,
                    InternalScript = option.InternalScript
                });
            }
        }

        internal static List<Event> PopulateFromLegacy(
            List<Model.LegacySerialization.Event> eventList)
        {
            return eventList.Select(legacyItem => new Event(legacyItem)).ToList();
        }
    }
}
