using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;
using System;

namespace FocusTreeManager.Model
{
    public class MutuallyExclusiveSetModel : ObservableObject, 
        ISupportsUndo, ISet, IEquatable<MutuallyExclusiveSetModel>
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
            if (focus1.X < focus2.X)
            {
                Focus1 = focus1;
                Focus2 = focus2;
            }
            else if (focus1.X >= focus2.X)
            {
                Focus2 = focus1;
                Focus1 = focus2;
            }
        }

        public void DeleteSetRelations()
        {
            Focus1.MutualyExclusive.Remove(this);
            Focus1 = null;
            Focus2.MutualyExclusive.Remove(this);
            Focus2 = null;
        }

        public bool Equals(MutuallyExclusiveSetModel obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Focus1 == Focus1 && obj.Focus2 == Focus2)
            {
                return true;
            }
            if (obj.Focus2 == Focus1 && obj.Focus1 == Focus2)
            {
                return true;
            }
            return false;
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return new ViewModelLocator().Main;
        }

        #endregion
    }
}
