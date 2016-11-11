using FocusTreeManager.CodeStructures;
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
                return (new ViewModelLocator()).Main.Project.getSpecificScriptList(ID).ContainerID;
            }
        }

        public Script InternalScript
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificScriptList(ID).InternalScript;
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
            (new ViewModelLocator()).Main.Project.getSpecificScriptList(ID).InternalScript =
                (new ViewModelLocator()).Scripter.ManagedScript;
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
