using FocusTreeManager.DataContract;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;

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
                var element = Project.Instance.getSpecificLocalisationMap(ID);
                return element != null ? element.ContainerID : null;
            }
        }

        public string ShortName
        {
            get
            {
                return Project.Instance.getSpecificLocalisationMap(ID).ShortName;
            }
            set
            {
                Project.Instance.getSpecificLocalisationMap(ID).ShortName = value;
                RaisePropertyChanged("ShortName");
            }
        }

        public ObservableCollection<LocaleModel> LocalisationMap
        {
            get
            {
                return Project.Instance.getSpecificLocalisationMap(ID).getLocalisationModelList();
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
            if (this.Filename == null)
            {
                return;
            }
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
        }
    }
}
