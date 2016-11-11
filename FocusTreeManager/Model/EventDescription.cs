using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [ProtoContract]
    public class EventDescription : ObservableObject
    {
        public string Text
        {
            get
            {
                string Id = "";
                try
                {
                    Id = internalScript.FindValue("text").Parse();
                }
                catch (Exception)
                {
                    Id = "eventid.d_descriptioname";
                }
                var locales = (new ViewModelLocator()).Main.Project.getLocalisationWithKey(Id);
                string translation = locales != null ? locales.translateKey(Id) : null;
                return translation != null ? translation : Id;
            }
        }

        [ProtoMember(1)]
        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                Set<Script>(() => this.InternalScript, ref this.internalScript, value);
                RaisePropertyChanged(() => Text);
            }
        }

        public EventDescription()
        {
        }

        public void setDefaults()
        {
            InternalScript = new Script();
            InternalScript.Analyse("text = namespace.count.d.desc_id");
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
