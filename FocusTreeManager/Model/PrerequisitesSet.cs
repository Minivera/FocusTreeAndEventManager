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
    public class PrerequisitesSet : ObservableObject, ISet
    {
        [ProtoMember(1, AsReference = true)]
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
            if (Focus == null)
            {
                //Already deleted, click on more than one line
                return;
            }
            Focus.Prerequisite.Remove(this);
            Focus = null;
            FociList.Clear();
        }

        public void assertInternalFocus(IEnumerable<Focus> fociList)
        {
            //Repair the main focus
            Focus = fociList.FirstOrDefault((f) => f.X == Focus.X && f.Y == Focus.Y);
            //Repair its parents
            List<Focus> realList = new List<Focus>();
            foreach (Focus item in this.FociList)
            {
                realList.Add(fociList.FirstOrDefault((f) => f.X == item.X && f.Y == item.Y));
            }
            this.FociList = realList;
        }
    }
}
