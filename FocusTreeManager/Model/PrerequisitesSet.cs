using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    public class PrerequisitesSet : ObservableObject, ISet
    { 
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
            Focus.Prerequisite.Remove(this);
            FociList.Clear();
        }
    }
}
