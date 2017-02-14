using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using System;

namespace FocusTreeManager.Model
{
    public class EventDescriptionModel : ObservableObject
    {
        public string Text
        {
            get
            {
                string Id = "";
                try
                {
                    Id = InternalScript.FindValue("text").Parse();
                }
                catch (Exception)
                {
                    Id = "eventid.d_descriptioname";
                }

                var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales != null ? locales.translateKey(Id) : null;
                return translation != null ? translation : Id;
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
                RaisePropertyChanged(() => Text);
            }
        }

        public EventDescriptionModel() { }

        public void EditDescScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(InternalScript,
                ScripterControlsViewModel.ScripterType.EventDescription);
            dialog.ShowDialog();
            internalScript = ViewModel.ManagedScript;
        }

        public void setDefaults()
        {
            internalScript = new Script();
            internalScript.Analyse("text = namespace.count.d.desc_id");
        }
    }
}
