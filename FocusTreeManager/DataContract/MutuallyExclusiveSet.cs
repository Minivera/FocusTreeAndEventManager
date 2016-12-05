using FocusTreeManager.Model;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "mutually_exclusive", Namespace = "focusesNs")]
    public class MutuallyExclusiveSet : ISet
    {
        [DataMember(Name = "focus_left", Order = 0)]
        public Focus Focus1 { get; set; }
        
        [DataMember(Name = "focus_right", Order = 1)]
        public Focus Focus2 { get; set; }

        public MutuallyExclusiveSetModel Model { get; private set; }

        public MutuallyExclusiveSet(Focus Focus1, Focus Focus2)
        {
            //Set leftmost Focus as Focus 1 and rightmost focus as focus 2
            if (Focus1.X < Focus2.X)
            {
                this.Focus1 = Focus1;
                this.Focus2 = Focus2;
            }
            else if(Focus1.X >= Focus2.X)
            {
                this.Focus2 = Focus1;
                this.Focus1 = Focus2;
            }
            Model = new MutuallyExclusiveSetModel(this);
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            Model = new MutuallyExclusiveSetModel(this);
        }

        public void DeleteSetRelations()
        {
            Focus1.MutualyExclusive.Remove(this);
            Focus1 = null;
            Focus2.MutualyExclusive.Remove(this);
            Focus2 = null;
        }
    }
}
