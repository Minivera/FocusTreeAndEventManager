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
    public class LocalisationContainer : ObservableObject
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
            }
        }

        [ProtoMember(2)]
        private string shortname;

        public string ShortName
        {
            get
            {
                return shortname;
            }
            set
            {
                Set<string>(() => this.ShortName, ref this.shortname, value);
            }
        }

        [ProtoMember(3)]
        public ObservableCollection<LocaleContent> LocalisationMap { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public LocalisationContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public LocalisationContainer(string filename)
        {
            ContainerID = filename;
            LocalisationMap = new ObservableCollection<LocaleContent>();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }
        
        public string translateKey(string key)
        {
            LocaleContent locale = LocalisationMap.SingleOrDefault((l) => l.Key.ToLower() == key.ToLower());
            return locale != null ? locale.Value : null;
        }
        
        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,"SendDeleteItemSignal"));
        }
    }
}
