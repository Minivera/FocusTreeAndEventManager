using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace FocusTreeManager.Model
{
    public class ScriptModel : ObservableObject
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
                var element = Project.Instance.getSpecificScriptList(ID);
                return element != null ? element.ContainerID : null;
            }
        }

        public Script InternalScript
        {
            get
            {
                return Project.Instance.getSpecificScriptList(ID).InternalScript;
            }
        }

        public RelayCommand<FrameworkElement> SaveScriptCommand { get; set; }

        public ScriptModel(Guid ID)
        {
            this.ID = ID;
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            SaveScriptCommand = new RelayCommand<FrameworkElement>(SaveScript);
        }

        private void SaveScript(FrameworkElement obj)
        {
            (new ViewModelLocator()).Scripter.SaveScript();
            Project.Instance.getSpecificScriptList(ID).InternalScript =
                (new ViewModelLocator()).Scripter.ManagedScript;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (this.Filename == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
        }
    }
}
