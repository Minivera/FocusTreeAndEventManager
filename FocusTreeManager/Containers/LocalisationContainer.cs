using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Containers
{
    [Serializable]
    public class LocalisationContainer : ObservableObject
    {
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
                Messenger.Default.Send(new NotificationMessage(this, "RenamedContainer"));
            }
        }

        public ObservableCollection<LocaleContent> LocalisationMap { get; set; }

        public LocalisationContainer(string filename)
        {
            ContainerID = filename;
            LocalisationMap = new ObservableCollection<LocaleContent>();
        }



        public string translateKey(string key)
        {
            LocaleContent locale = LocalisationMap.SingleOrDefault((l) => l.Key.ToLower() == key.ToLower());
            return locale != null ? locale.Value : null;
        }
    }
}
