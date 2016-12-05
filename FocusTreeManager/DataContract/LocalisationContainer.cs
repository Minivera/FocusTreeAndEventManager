using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(LocaleContent))]
    [DataContract(Name = "locale_container")]
    public class LocalisationContainer
    {
        [DataMember(Name = "guid_id", Order = 1)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 2)]
        public string ContainerID { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string ShortName { get; set; }

        [DataMember(Name = "locales", Order = 4)]
        public List<LocaleContent> LocalisationMap { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public LocalisationContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public LocalisationContainer(Containers.LegacySerialization.LocalisationContainer legacyItem)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
            ContainerID = legacyItem.ContainerID;
            ShortName = legacyItem­.ShortName;
            LocalisationMap = LocaleContent.PopulateFromLegacy(legacyItem.LocalisationMap.ToList());
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        public ObservableCollection<LocaleModel> getLocalisationModelList()
        {
            ObservableCollection<LocaleModel> list = new ObservableCollection<LocaleModel>();
            foreach (LocaleContent item in LocalisationMap)
            {
                list.Add(new LocaleModel(item));
            }
            return list;
        }

        internal static List<LocalisationContainer> PopulateFromLegacy(
            List<Containers.LegacySerialization.LocalisationContainer> localisationList)
        {
            List<LocalisationContainer> list = new List<LocalisationContainer>();
            foreach (Containers.LegacySerialization.LocalisationContainer legacyItem in localisationList)
            {
                list.Add(new LocalisationContainer(legacyItem));
            }
            return list;
        }

        public LocalisationContainer(string filename)
        {
            ContainerID = filename;
            LocalisationMap = new List<LocaleContent>();
            IdentifierID = Guid.NewGuid();
        }
        
        public string translateKey(string key)
        {
            LocaleContent locale = LocalisationMap.SingleOrDefault((l) => l.Key.ToLower() == key.ToLower());
            return locale != null ? locale.Value : null;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, 
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
    }
}
