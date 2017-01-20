using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System;
using FocusTreeManager.DataContract;
using System.Collections.ObjectModel;
using MonitoredUndo;
using System.Collections.Specialized;
using FocusTreeManager.ViewModel;

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
            Focus = linkedFocus;
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

        #region Undo/Redo

        void FociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "FociList",
                this.FociList, e, "FociList Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
