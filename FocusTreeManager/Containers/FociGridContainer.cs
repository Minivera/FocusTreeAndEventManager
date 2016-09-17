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
    public class FociGridContainer : ObservableObject
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

        public ObservableCollection<Focus> FociList { get; set; }

        public FociGridContainer(string filename)
        {
            ContainerID = filename;
            FociList = new ObservableCollection<Focus>();
        }
    }
}
