using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;

namespace FocusTreeManager.Model
{
    public class EventOptionModel : ObservableObject
    {
        public EventOption DataContract { get; set; }

        public string Name
        {
            get
            {
                return DataContract.Name;
            }
            set
            {
                DataContract.Name = value;
                RaisePropertyChanged(() => Name);
                RaisePropertyChanged(() => Text);
            }
        }

        public string Text
        {
            get
            {
                var locales = Project.Instance.getLocalisationWithKey(Name);
                string translation = locales != null ? locales.translateKey(Name) : null;
                return translation != null ? translation : Name;
            }
        }

        public Script InternalScript
        {
            get
            {
                return DataContract.InternalScript;
            }
            set
            {
                DataContract.InternalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        public EventOptionModel(EventOption linkedContract)
        {
            DataContract = linkedContract;
        }

        public void setDefaults()
        {
            Name = "namespace.count.a";
            InternalScript = new Script();
        }

        public void EditOptionScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(InternalScript,
                ScripterControlsViewModel.ScripterType.EventOption);
            dialog.ShowDialog();
            InternalScript = ViewModel.ManagedScript;
        }
    }
}
