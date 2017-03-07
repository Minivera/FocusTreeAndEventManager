using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;

namespace FocusTreeManager.Model
{
    public class LocaleModel : ObservableObject, ISupportsUndo
    {
        private string key;

        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                if (value == key)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Key", key, value, "Key Changed");
                key = value;
                RaisePropertyChanged(() => Key);
            }
        }

        private string value;

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value == this.value)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Key", value, value, "Key Changed");
                this.value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
