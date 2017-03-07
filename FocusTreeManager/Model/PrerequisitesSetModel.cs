using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using MonitoredUndo;
using System.Collections.Specialized;
using FocusTreeManager.ViewModel;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class PrerequisitesSetModel : ObservableObject, ISupportsUndo, ISet
    {
        private FocusModel focus;

        public FocusModel Focus
        {
            get
            {
                return focus;
            }
            set
            {
                if (value == focus)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Focus", focus, value, "Focus Changed");
                focus = value;
                RaisePropertyChanged(() => Focus);
            }
        }
        
        public ObservableCollection<FocusModel> FociList { get; set; }

        public PrerequisitesSetModel(FocusModel linkedFocus)
        {
            FociList = new ObservableCollection<FocusModel>();
            FociList.CollectionChanged += FociList_CollectionChanged;
            focus = linkedFocus;
        }

        public bool isRequired()
        {
            return FociList.Count > 1;
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
            foreach (FocusModel item in FociList.ToList())
            {
                FociList.Remove(item);
            }
        }

        #region Undo/Redo

        private void FociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "FociList",
                FociList, e, "FociList Changed");
        }

        public object GetUndoRoot()
        {
            return new ViewModelLocator().Main;
        }

        #endregion
    }
}
