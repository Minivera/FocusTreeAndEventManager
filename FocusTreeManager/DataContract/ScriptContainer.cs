using FocusTreeManager.CodeStructures;
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [DataContract(Name = "script_container")]
    public class ScriptContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "script", Order = 2)]
        public Script InternalScript { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public ScriptContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public ScriptContainer(string filename)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            ContainerID = filename;
            InternalScript = new Script();
            IdentifierID = Guid.NewGuid();
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        public ScriptContainer(Containers.LegacySerialization.ScriptContainer legacyItem)
        {
            ContainerID = legacyItem.ContainerID;
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
            IdentifierID = legacyItem.IdentifierID;
        }

        internal static List<ScriptContainer> PopulateFromLegacy(
            List<Containers.LegacySerialization.ScriptContainer> scriptList)
        {
            List<ScriptContainer> list = new List<ScriptContainer>();
            foreach (Containers.LegacySerialization.ScriptContainer legacyItem in scriptList)
            {
                list.Add(new ScriptContainer(legacyItem));
            }
            return list;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
    }
}
