using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;

namespace FocusTreeManager.Containers
{
    public class FociGridContainer : ObservableObject
    {
        public Guid IdentifierID { get; private set; }
        
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
        
        private string containerTag;

        public string TAG
        {
            get
            {
                return containerTag;
            }
            set
            {
                Set<string>(() => this.TAG, ref this.containerTag, value);
            }
        }
        
        public ObservableCollection<Focus> FociList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public FociGridContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public FociGridContainer(LegacySerialization.FociGridContainer legacyItem)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
            ContainerID = legacyItem.ContainerID;
            TAG = legacyItem.TAG;
            FociList = Focus.PopulateFromLegacy(legacyItem.FociList);
        }

        internal static ObservableCollection<FociGridContainer> PopulateFromLegacy
            (ObservableCollection<LegacySerialization.FociGridContainer> fociContainerList)
        {
            ObservableCollection<FociGridContainer> list = new ObservableCollection<FociGridContainer>();
            foreach (LegacySerialization.FociGridContainer legacyItem in fociContainerList)
            {
                list.Add(new FociGridContainer(legacyItem));
            }
            return list;
        }

        public FociGridContainer(string filename)
        {
            ContainerID = filename;
            FociList = new ObservableCollection<Focus>();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, "SendDeleteItemSignal"));
        }
    }
}
