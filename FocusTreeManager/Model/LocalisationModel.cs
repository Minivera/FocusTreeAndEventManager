using FocusTreeManager.Containers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    public class LocalisationModel : ObservableObject
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        public string Filename
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificLocalisationMap(ID).ContainerID;
            }
        }

        public string ShortName
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificLocalisationMap(ID).ShortName;
            }
            set
            {
                (new ViewModelLocator()).Main.Project.getSpecificLocalisationMap(ID).ShortName = value;
                RaisePropertyChanged("ShortName");
            }
        }

        public ObservableCollection<LocaleContent> LocalisationMap
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificLocalisationMap(ID).LocalisationMap;
            }
        }

        public LocalisationModel(Guid ID)
        {
            this.ID = ID;
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
        }
    }
}
