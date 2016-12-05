using FocusTreeManager.DataContract;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class MutuallyExclusiveSetModel : ObservableObject
    {
        public MutuallyExclusiveSet DataContract { get; set; }

        public FocusModel Focus1
        {
            get
            {
                return DataContract.Focus1.Model;
            }
            set
            {
                DataContract.Focus1 = value.DataContract;
                RaisePropertyChanged(() => Focus1);
            }
        }
        
        public FocusModel Focus2
        {
            get
            {
                return DataContract.Focus2.Model;
            }
            set
            {
                DataContract.Focus2 = value.DataContract;
                RaisePropertyChanged(() => Focus2);
            }
        }

        public MutuallyExclusiveSetModel(MutuallyExclusiveSet linkedContract)
        {
            DataContract = linkedContract;
        }
    }
}
