using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using MonitoredUndo;

namespace FocusTreeManager.Model
{
    public class EventOptionModel : ObservableObject, ISupportsUndo
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == name)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Name", name, value, "Name Changed");
                name = value;
                RaisePropertyChanged(() => Name);
                RaisePropertyChanged(() => Text);
            }
        }

        public string Text
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.getLocalisationWithKey(Name);
                string translation = locales != null ? locales.translateKey(Name) : null;
                return translation != null ? translation : Name;
            }
        }

        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                if (value == internalScript)
                {
                    return;
                }
                internalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        public EventOptionModel() { }

        public void setDefaults()
        {
            name = "namespace.count.a";
            internalScript = new Script();
        }

        public void EditOptionScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(InternalScript,
                ScripterControlsViewModel.ScripterType.EventOption);
            dialog.ShowDialog();
            internalScript = ViewModel.ManagedScript;
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
