using GalaSoft.MvvmLight;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [ProtoContract]
    [ProtoInclude(500, typeof(ISet))]
    public class MutuallyExclusiveSet : ObservableObject, ISet
    {
        [ProtoMember(1)]
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

        [ProtoMember(2)]
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

        public MutuallyExclusiveSet() { }

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
    }
}
