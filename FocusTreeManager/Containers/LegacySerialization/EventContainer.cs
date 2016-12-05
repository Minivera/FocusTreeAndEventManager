using FocusTreeManager.Model.LegacySerialization;
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

namespace FocusTreeManager.Containers.LegacySerialization
{
    [ProtoContract]
    public class EventContainer : ObservableObject
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
        private string eventNamespace;

        public string EventNamespace
        {
            get
            {
                return eventNamespace;
            }
            set
            {
                Set<string>(() => this.EventNamespace, ref this.eventNamespace, value);
            }
        }

        [ProtoMember(3)]
        public ObservableCollection<Event> EventList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public EventContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public EventContainer(string filename)
        {
            ContainerID = filename;
            EventList = new ObservableCollection<Event>();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }
        
        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                "SendDeleteItemSignal"));
        }
    }
}
