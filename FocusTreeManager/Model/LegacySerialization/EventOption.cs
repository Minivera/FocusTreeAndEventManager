using FocusTreeManager.CodeStructures.LegacySerialization;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using ProtoBuf;

namespace FocusTreeManager.Model.LegacySerialization
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
                Set<string>(() => Name, ref name, value);
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
                Set<Script>(() => InternalScript, ref internalScript, value);
            }
        }

        public EventOption()
        {
        }
    }
}
