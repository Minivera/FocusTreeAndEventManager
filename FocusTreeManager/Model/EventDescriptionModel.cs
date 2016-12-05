using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

namespace FocusTreeManager.Model
{
    public class EventDescriptionModel : ObservableObject
    {
        public EventDescription DataContract { get; set; }

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
                var locales = Project.Instance.getLocalisationWithKey(Id);
                string translation = locales != null ? locales.translateKey(Id) : null;
                return translation != null ? translation : Id;
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
                RaisePropertyChanged(() => Text);
            }
        }

        public EventDescriptionModel(EventDescription linkedContract)
        {
            DataContract = linkedContract;
        }

        public void EditDescScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(InternalScript,
                ScripterControlsViewModel.ScripterType.EventDescription);
            dialog.ShowDialog();
            InternalScript = ViewModel.ManagedScript;
        }
    }
}
