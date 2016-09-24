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
    public class PrerequisitesSet : ObservableObject, ISet
    {
        [ProtoMember(1)]
        private Focus focus;

        public Focus Focus
        {
            get
            {
                return focus;
            }
            set
            {
                Set<Focus>(() => this.Focus, ref this.focus, value);
            }
        }

        [ProtoMember(2)]
        private List<Focus> fociList;

        public List<Focus> FociList
        {
            get
            {
                return fociList;
            }
            set
            {
                Set<List<Focus>>(() => this.FociList, ref this.fociList, value);
            }
        }

        public PrerequisitesSet() { }

        public PrerequisitesSet(Focus focus)
        {
            this.Focus = focus;
            FociList = new List<Focus>();
        }

        public bool isRequired()
        {
            return (FociList.Count > 1);
        }

        public void DeleteSetRelations()
        {
            Focus.Prerequisite.Remove(this);
            FociList.Clear();
        }
    }
}
