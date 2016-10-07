using GalaSoft.MvvmLight;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(500, typeof(ISet))]
    public class MutuallyExclusiveSet : ObservableObject, ISet
    {
        [ProtoMember(1, AsReference = true)]
        private Focus focus1;

        public Focus Focus1
        {
            get
            {
                return focus1;
            }
            set
            {
                Set<Focus>(() => this.Focus1, ref this.focus1, value);
            }
        }

        [ProtoMember(2, AsReference = true)]
        private Focus focus2;

        public Focus Focus2
        {
            get
            {
                return focus2;
            }
            set
            {
                Set<Focus>(() => this.Focus2, ref this.focus2, value);
            }
        }

        public MutuallyExclusiveSet(Focus Focus1, Focus Focus2)
        {
            //Set leftmost Focus as Focus 1 and rightmost focus as focus 2
            if (Focus1.FocusLeft.X < Focus2.FocusLeft.X)
            {
                this.Focus1 = Focus1;
                this.Focus2 = Focus2;
            }
            else if(Focus1.FocusLeft.X >= Focus2.FocusLeft.X)
            {
                this.Focus2 = Focus1;
                this.Focus1 = Focus2;
            }
        }

        public void DeleteSetRelations()
        {
            Focus1.MutualyExclusive.Remove(this);
            Focus1 = null;
            Focus2.MutualyExclusive.Remove(this);
            Focus2 = null;
        }

        public void assertInternalFocus(IEnumerable<Focus> fociList)
        {
            //Repair Focus 1, get the reference in the list
            Focus1 = fociList.FirstOrDefault((f) => f.X == Focus1.X && f.Y == Focus1.Y);
            //Repair Focus 2, get the reference in the list
            Focus2 = fociList.FirstOrDefault((f) => f.X == Focus2.X && f.Y == Focus2.Y);
        }
    }
}
