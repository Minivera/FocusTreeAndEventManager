using FocusTreeManager.DataContract;
using GalaSoft.MvvmLight;

namespace FocusTreeManager.Model
{
    public class LocaleModel : ObservableObject
    {
        public LocaleContent DataContract { get; set; }

        public string Key
        {
            get
            {
                return DataContract.Key;
            }
            set
            {
                DataContract.Key = value;
                RaisePropertyChanged(() => Key);
            }
        }

        public string Value
        {
            get
            {
                return DataContract.Value;
            }
            set
            {
                DataContract.Value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public LocaleModel(LocaleContent linkedContract)
        {
            DataContract = linkedContract;
        }
    }
}
