using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "foci_container", Namespace = "focusesNs")]
    public class FociGridContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "tag", Order = 2)]
        public string TAG { get; set; }

        [DataMember(Name = "foci", Order = 3)]
        public List<Focus> FociList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public FociGridContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public FociGridContainer(Containers.LegacySerialization.FociGridContainer legacyItem)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = legacyItem.IdentifierID;
            ContainerID = legacyItem.ContainerID;
            TAG = legacyItem.TAG;
            FociList = Focus.PopulateFromLegacy(legacyItem.FociList.ToList());
        }

        public FociGridContainer(string filename)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            ContainerID = filename;
            FociList = new List<Focus>();
            IdentifierID = Guid.NewGuid();
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
        }

        internal static List<FociGridContainer> PopulateFromLegacy
            (List<Containers.LegacySerialization.FociGridContainer> fociContainerList)
        {
            List<FociGridContainer> list = new List<FociGridContainer>();
            foreach (Containers.LegacySerialization.FociGridContainer legacyItem in fociContainerList)
            {
                list.Add(new FociGridContainer(legacyItem));
            }
            return list;
        }

        public List<FocusModel> getFocusModelList()
        {
            List<FocusModel> list = new List<FocusModel>();
            foreach (Focus item in FociList)
            {
                list.Add(item.Model);
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
