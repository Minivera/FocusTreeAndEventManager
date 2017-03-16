using FocusTreeManager.CodeStructures;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [DataContract(Name = "event_option")]
    public class EventOption
    {
        [DataMember(Name = "name", Order = 0)]
        public string Name { get; set; }

        [DataMember(Name = "script", Order = 1)]
        public Script InternalScript { get; set; }

        public EventOption()
        {
        }

        public void setDefaults()
        {
            Name = "namespace.count.a";
            InternalScript = new Script();
        }
    }
}
