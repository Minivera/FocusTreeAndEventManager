using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Runtime.Serialization;
using FocusTreeManager.Containers.LegacySerialization;
using System.Collections.ObjectModel;

namespace FocusTreeManager.Containers
{
    [KnownType(typeof(Script))]
    [DataContract(Name = "script_container")]
    public class ScriptContainer : ObservableObject
    {
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "id", Order = 0)]
        private string containerID;

        public string ContainerID
        {
            get
            {
                return containerID;
            }
            set
            {
                Set<string>(() => this.ContainerID, ref this.containerID, value);
                Messenger.Default.Send(new NotificationMessage("ContainerRenamed"));
            }
        }

        [DataMember(Name = "script", Order = 1)]
        public Script InternalScript { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public ScriptContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public ScriptContainer(string filename)
        {
            ContainerID = filename;
            InternalScript = new Script();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public ScriptContainer(LegacySerialization.ScriptContainer legacyItem)
        {
            ContainerID = legacyItem.ContainerID;
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }

        internal static ObservableCollection<ScriptContainer> PopulateFromLegacy(
            ObservableCollection<LegacySerialization.ScriptContainer> scriptList)
        {
            ObservableCollection<ScriptContainer> list = new ObservableCollection<ScriptContainer>();
            foreach (LegacySerialization.ScriptContainer legacyItem in scriptList)
            {
                list.Add(new ScriptContainer(legacyItem));
            }
            return list;
        }
    }
}
