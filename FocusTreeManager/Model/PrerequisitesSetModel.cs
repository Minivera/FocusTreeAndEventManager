using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System;
using FocusTreeManager.DataContract;
using System.Collections.ObjectModel;

namespace FocusTreeManager.Model
{
    public class PrerequisitesSetModel : ObservableObject
    {
        public PrerequisitesSet DataContract { get; set; }

        public FocusModel Focus
        {
            get
            {
                return DataContract.Focus.Model;
            }
            set
            {
                DataContract.Focus = value.DataContract;
                RaisePropertyChanged(() => Focus);
            }
        }
        
        public List<FocusModel> FociList
        {
            get
            {
                return DataContract.getFocusModelList();
            }
        }

        public PrerequisitesSetModel(PrerequisitesSet linkedContract)
        {
            DataContract = linkedContract;
        }

        public bool isRequired()
        {
            return (FociList.Count > 1);
        }
    }
}
