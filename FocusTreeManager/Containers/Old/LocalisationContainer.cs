using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using FocusTreeManager.Containers.LegacySerialization;

namespace FocusTreeManager.Containers
{
    [KnownType(typeof(LocaleContent))]
    [DataContract(Name = "locale_container")]
    public class LocalisationContainer : ObservableObject
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

        [DataMember(Name = "name", Order = 1)]
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

        [DataMember(Name = "locales", Order = 2)]
        public ObservableCollection<LocaleContent> LocalisationMap { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public LocalisationContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public LocalisationContainer(LegacySerialization.LocalisationContainer legacyItem)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
            ContainerID = legacyItem.ContainerID;
            ShortName = legacyItem­.ShortName;
            LocalisationMap = LocaleContent.PopulateFromLegacy(legacyItem.LocalisationMap);
        }

        internal static ObservableCollection<LocalisationContainer> PopulateFromLegacy(
            ObservableCollection<LegacySerialization.LocalisationContainer> localisationList)
        {
            ObservableCollection<LocalisationContainer> list = new ObservableCollection<LocalisationContainer>();
            foreach (LegacySerialization.LocalisationContainer legacyItem in localisationList)
            {
                list.Add(new LocalisationContainer(legacyItem));
            }
            return list;
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
