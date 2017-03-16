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
    }
}
