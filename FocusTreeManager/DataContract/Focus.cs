using System.Runtime.Serialization;
using System.Collections.Generic;
using FocusTreeManager.CodeStructures;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [KnownType(typeof(PrerequisitesSet))]
    [KnownType(typeof(MutuallyExclusiveSet))]
    [DataContract(Name = "focus", Namespace = "focusesNs")]
    public class Focus
    {

        [DataMember(Name = "image", Order = 0)]
        public string Image { get; set; }

        [DataMember(Name = "name", Order = 1)]
        public string UniqueName { get; set; }

        [DataMember(Name = "x", Order = 2)]
        public int X { get; set; }

        [DataMember(Name = "y", Order = 3)]
        public int Y { get; set; }

        [DataMember(Name = "cost", Order = 4)]
        public double Cost { get; set; }

        [DataMember(Name = "script", Order = 5)]
        public Script InternalScript { get; set; }

        [DataMember(Name = "prerequisites", Order = 6)]
        public List<PrerequisitesSet> Prerequisite { get; set; }

        [DataMember(Name = "mutually_exclusives", Order = 7)]
        public List<MutuallyExclusiveSet> MutualyExclusive { get; set; }

        [DataMember(Name = "text", Order = 8)]
        public string Text { get; set; }

        [DataMember(Name = "relativeTo", Order = 9)]
        public Focus RelativeTo { get; set; }

        [DataMember(Name = "note", Order = 10)]
        public string Note { get; set; }

        public Focus()
        {
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
        }
    }
}
