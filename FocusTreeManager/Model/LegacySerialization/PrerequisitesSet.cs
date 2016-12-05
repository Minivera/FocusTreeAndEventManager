using GalaSoft.MvvmLight;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract(SkipConstructor = true)]
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

        public void DeleteSetRelations()
        {
            throw new NotImplementedException();
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
