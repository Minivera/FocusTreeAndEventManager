using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Views;
using System.Windows.Controls;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProjectViewViewModel : ViewModelBase
    {
        public ProjectModel Project
        {
            get
            {
                return (new ViewModelLocator()).Main.Project;
            }
        }

        public RelayCommand AddElementCommand { get; private set; }

        public RelayCommand DeleteElementMenuCommand { get; set; }

        public RelayCommand EditElementMenuCommand { get; set; }

        public RelayCommand<ObservableObject> OpenFileCommand { get; private set; }

        public object SelectedItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddElementCommand = new RelayCommand(AddElement);
            OpenFileCommand = new RelayCommand<ObservableObject>(OpenFile);
            DeleteElementMenuCommand = new RelayCommand(DeleteFile, CanExecuteOnFile);
            EditElementMenuCommand = new RelayCommand(EditFile, CanExecuteOnFile);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement()
        {
            FileManager dialog = new FileManager(ModeType.Create);
            dialog.ShowDialog();
            ObservableObject File = (new ViewModelLocator()).FileManager.File;
            if (File != null)
            {
                if (File is FocusGridModel)
                {
                    (new ViewModelLocator()).Main.Project.fociList.Add((FocusGridModel)File);
                    RaisePropertyChanged("fociList");
                }
                else if (File is LocalisationModel)
                {
                    (new ViewModelLocator()).Main.Project.localisationList.Add((LocalisationModel)File);
                    //Check if first, if yes, set as default
                    if ((new ViewModelLocator()).Main.Project.DefaultLocale == null)
                    {
                        (new ViewModelLocator()).Main.Project.DefaultLocale = (LocalisationModel)File;
                    }
                    RaisePropertyChanged("localisationList");
                }
                else if (File is EventTabModel)
                {
                    (new ViewModelLocator()).Main.Project.eventList.Add((EventTabModel)File);
                    RaisePropertyChanged("eventList");
                }
                else if(File is ScriptModel)
                {
                    (new ViewModelLocator()).Main.Project.scriptList.Add((ScriptModel)File);
                    RaisePropertyChanged("scriptList");
                }
            }
        }

        private void DeleteElement(object item)
        {
            if (item is FocusGridModel)
            {
                (new ViewModelLocator()).Main.Project.fociList.Remove((FocusGridModel)item);
                RaisePropertyChanged("fociContainerList");
            }
            else if (item is LocalisationModel)
            {
                (new ViewModelLocator()).Main.Project.localisationList.Remove((LocalisationModel)item);
                RaisePropertyChanged("localisationList");
            }
            else if (item is EventTabModel)
            {
                (new ViewModelLocator()).Main.Project.eventList.Remove((EventTabModel)item);
                RaisePropertyChanged("eventList");
            }
            else if (item is ScriptModel)
            {
                (new ViewModelLocator()).Main.Project.scriptList.Remove((ScriptModel)item);
                RaisePropertyChanged("scriptList");
            }
        }

        private void EditElement(object obj)
        {
            FileManager dialog = new FileManager(ModeType.Edit);
            if (obj is FocusGridModel)
            {
                var item = obj as FocusGridModel;
                (new ViewModelLocator()).FileManager.File = new FocusGridModel(item.VisibleName)
                {
                    TAG = item.TAG,
                    AdditionnalMods = item.AdditionnalMods
                };
                dialog.ShowDialog();
                if ((new ViewModelLocator()).FileManager.File != null)
                {
                    var newItem = (new ViewModelLocator()).FileManager.File as FocusGridModel;
                    item.VisibleName = newItem.VisibleName;
                    item.TAG = newItem.TAG;
                    item.AdditionnalMods = newItem.AdditionnalMods;
                }
            }
            else if (obj is LocalisationModel)
            {
                var item = obj as LocalisationModel;
                (new ViewModelLocator()).FileManager.File = new LocalisationModel(item.VisibleName)
                {
                    LanguageName = item.LanguageName
                };
                dialog.ShowDialog();
                if ((new ViewModelLocator()).FileManager.File != null)
                {
                    var newItem = (new ViewModelLocator()).FileManager.File as LocalisationModel;
                    item.VisibleName = newItem.VisibleName;
                    item.LanguageName = newItem.LanguageName;
                }
            }
            else if (obj is EventTabModel)
            {
                var item = obj as EventTabModel;
                (new ViewModelLocator()).FileManager.File = new EventTabModel(item.VisibleName)
                {
                    EventNamespace = item.EventNamespace
                };
                dialog.ShowDialog();
                if ((new ViewModelLocator()).FileManager.File != null)
                {
                    var newItem = (new ViewModelLocator()).FileManager.File as EventTabModel;
                    item.VisibleName = newItem.VisibleName;
                    item.EventNamespace = newItem.EventNamespace;
                }
            }
            else if (obj is ScriptModel)
            {
                var item = obj as ScriptModel;
                (new ViewModelLocator()).FileManager.File = new ScriptModel(item.VisibleName);
                dialog.ShowDialog();
                if ((new ViewModelLocator()).FileManager.File != null)
                {
                    var newItem = (new ViewModelLocator()).FileManager.File as ScriptModel;
                    item.VisibleName = newItem.VisibleName;
                }
            }
        }

        private void OpenFile(ObservableObject item)
        {
            if (item is FocusGridModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenFocusTree"));
            }
            else if (item is LocalisationModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenLocalisation"));
            }
            else if (item is EventTabModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenEventList"));
            }
            else if (item is ScriptModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, (new ViewModelLocator()).Main, "OpenScriptList"));
            }
        }

        public void EditFile()
        {
            EditElement(SelectedItem);
        }

        public void DeleteFile()
        {
            DeleteElement(SelectedItem);
        }

        public bool CanExecuteOnFile()
        {
            if (SelectedItem != null)
            {
                return true;
            }
            return false;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "SendDeleteItemSignal")
            {
                DeleteElement(msg.Sender);
            }
            if (msg.Notification == "SendEditItemSignal")
            {
                EditElement(msg.Sender);
            }
            if (msg.Notification == "RefreshProjectViewer")
            {
                RaisePropertyChanged(() => Project);
                RaisePropertyChanged(() => Project.fociList);
                RaisePropertyChanged(() => Project.localisationList);
                RaisePropertyChanged(() => Project.eventList);
                RaisePropertyChanged(() => Project.scriptList);
            }
        }
    }
}