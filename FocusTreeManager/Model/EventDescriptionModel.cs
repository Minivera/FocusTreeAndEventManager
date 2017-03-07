using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using System;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Model
{
    public class EventDescriptionModel : ObservableObject
    {
        public string Text
        {
            get
            {
                string Id;
                try
                {
                    Id = InternalScript.FindValue("text").Parse();
                }
                catch (Exception)
                {
                    Id = "eventid.d_descriptioname";
                }

                LocalisationModel locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales?.translateKey(Id);
                return translation ?? Id;
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
