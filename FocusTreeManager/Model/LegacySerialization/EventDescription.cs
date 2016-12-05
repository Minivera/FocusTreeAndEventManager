using FocusTreeManager.CodeStructures.LegacySerialization;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using ProtoBuf;
using System;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract]
    public class EventDescription : ObservableObject
    {
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
            }
        }

        public EventDescription()
        {
        }
    }
}
