using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class LocalisationModel : ObservableObject, ISupportsUndo
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (value == filename)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Filename", filename, value, "Filename Changed");
                filename = value;
                RaisePropertyChanged(() => Filename);
            }
        }

        private string shortname;

        public string Shortname
        {
            get
            {
                return shortname;
            }
            set
            {
                if (value == shortname)
                {
                    return;
                }
                shortname = value;
                RaisePropertyChanged(() => Shortname);
            }
        }

        public ObservableCollection<LocaleModel> LocalisationMap { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public LocalisationModel(string Filename)
        {
            filename = Filename;
            this.ID = Guid.NewGuid();
            LocalisationMap = new ObservableCollection<LocaleModel>();
            LocalisationMap.CollectionChanged += LocalisationMap_CollectionChanged;
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Commands
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
        }

        public LocalisationModel(LocalisationContainer container)
        {
            this.ID = container.IdentifierID;
            filename = container.ContainerID;
            shortname = container.ShortName;
            LocalisationMap = new ObservableCollection<LocaleModel>();
            foreach (LocaleContent content in container.LocalisationMap)
            {
                LocalisationMap.Add(new LocaleModel() { Key = content.Key, Value = content.Value});
            }
            LocalisationMap.CollectionChanged += LocalisationMap_CollectionChanged;
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Commands
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
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

        public string translateKey(string key)
        {
            LocaleModel locale = LocalisationMap.FirstOrDefault((l) => 
                                    l.Key.ToLower() == key.ToLower());
            return locale != null ? locale.Value : null;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendEditItemSignal"));
        }

        #region Undo/Redo

        void LocalisationMap_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "LocalisationMap",
                this.LocalisationMap, e, "LocalisationMap Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
