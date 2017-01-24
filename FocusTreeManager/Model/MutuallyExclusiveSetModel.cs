using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class MutuallyExclusiveSetModel : ObservableObject, ISupportsUndo, ISet
    {
        private FocusModel focus1;

        public FocusModel Focus1
        {
            get
            {
                return focus1;
            }
            set
            {
                if (value == focus1)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Focus1", focus1, value, "Focus1 Changed");
                focus1 = value;
                RaisePropertyChanged(() => Focus1);
            }
        }

        private FocusModel focus2;

        public FocusModel Focus2
        {
            get
            {
                return focus2;
            }
            set
            {
                if (value == focus2)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Focus2", focus2, value, "Focus2 Changed");
                focus2 = value;
                RaisePropertyChanged(() => Focus2);
            }
        }

        public MutuallyExclusiveSetModel(FocusModel focus1, FocusModel focus2)
        {
            //Set leftmost Focus as Focus 1 and rightmost focus as focus 2
            if (Focus1.X < Focus2.X)
            {
                this.Focus1 = Focus1;
                this.Focus2 = Focus2;
            }
            else if (Focus1.X >= Focus2.X)
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

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
