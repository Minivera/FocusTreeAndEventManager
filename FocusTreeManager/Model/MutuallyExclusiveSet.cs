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
            this.Focus1 = Focus1;
            this.Focus2 = Focus2;
        }

        public void DeleteSetRelations()
        {
            Focus1.MutualyExclusive.Remove(this);
            Focus1 = null;
            Focus2.MutualyExclusive.Remove(this);
            Focus2 = null;
        }

        public bool assertInternalFocus(Focus focus, bool set = true)
        {
            //If it is not the same as focus 1, but the same coordinates
            if (this.Focus1 != focus && (this.Focus1.X == focus.X && this.Focus1.Y == focus.Y))
            {
                if (set)
                {
                    this.Focus1 = focus;
                }
                return false;
            }
            //If it is not the same as focus 2, but the same coordinates
            else if (this.Focus2 != focus && (this.Focus2.X == focus.X && this.Focus2.Y == focus.Y))
            {
                if (set)
                {
                    this.Focus2 = focus;
                }
                return false;
            }
            return true;
        }
    }
}
