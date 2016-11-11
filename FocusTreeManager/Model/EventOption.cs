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
    public class EventOption : ObservableObject
    {
        [ProtoMember(1)]
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Set<string>(() => this.Name, ref this.name, value);
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

        [ProtoMember(2)]
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
            }
        }

        public EventOption()
        {
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
