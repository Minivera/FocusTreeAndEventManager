using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "mutually_exclusive", Namespace = "focusesNs")]
    public class MutuallyExclusiveSet
    {
        [DataMember(Name = "focus_left", Order = 0)]
        public Focus Focus1 { get; set; }
        
        [DataMember(Name = "focus_right", Order = 1)]
        public Focus Focus2 { get; set; }

        public MutuallyExclusiveSet(Focus Focus1, Focus Focus2)
        {
            this.Focus1 = Focus1;
            this.Focus2 = Focus2;
        }
    }
}
