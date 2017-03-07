using FocusTreeManager.CodeStructures;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [DataContract(Name = "event_description")]
    public class EventDescription
    {
        [DataMember(Name = "script", Order = 0)]
        public Script InternalScript { get; set; }

        public EventDescription()
        {
        }

        public EventDescription(Model.LegacySerialization.EventDescription legacyItem)
        {
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
        }

        internal static List<EventDescription> PopulateFromLegacy(
            List<Model.LegacySerialization.EventDescription> descriptions)
        {
            return descriptions.Select(legacyItem => new EventDescription(legacyItem)).ToList();
        }
    }
}
