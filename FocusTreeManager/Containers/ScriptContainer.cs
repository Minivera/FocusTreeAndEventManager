using FocusTreeManager.CodeStructures;
using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Containers
{
    [ProtoContract]
    public class ScriptContainer : ObservableObject
    {
        public Guid IdentifierID { get; private set; }

        [ProtoMember(1)]
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

        [ProtoMember(2)]
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
        
        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
    }
}
