using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Views;
using FocusTreeManager.Model.TabModels;
using MonitoredUndo;

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
        public ProjectModel Project => new ViewModelLocator().Main.Project;

        public RelayCommand AddElementCommand { get; private set; }

        public RelayCommand DeleteElementMenuCommand { get; set; }

        public RelayCommand EditElementMenuCommand { get; set; }

        public RelayCommand CopyElementMenuCommand { get; set; }

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
            CopyElementMenuCommand = new RelayCommand(CopyFile, CanExecuteOnFile);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void AddElement()
        {
            FileManager dialog = new FileManager(ModeType.Create);
            dialog.ShowDialog();
            ObservableObject File = (new ViewModelLocator()).FileManager.File;
            if (File == null) return;
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("AddAnyFile", false);
            FocusGridModel file = File as FocusGridModel;
            if (file != null)
            {
                ProjectModel project = new ViewModelLocator().Main.Project;
                project.fociList.Add(file);
                RaisePropertyChanged(() => project.fociList);
            }
            else if (File is LocalisationModel)
            {
                ProjectModel project = new ViewModelLocator().Main.Project;
                project.localisationList.Add((LocalisationModel)File);
                //Check if first, if yes, set as default
                if (project.DefaultLocale == null)
                {
                    project.DefaultLocale = (LocalisationModel)File;
                }
                RaisePropertyChanged(() => project.localisationList);
            }
            else if (File is EventTabModel)
            {
                ProjectModel project = new ViewModelLocator().Main.Project;
                project.eventList.Add((EventTabModel)File);
                RaisePropertyChanged(() => project.eventList);
            }
            else if(File is ScriptModel)
            {
                ProjectModel project = new ViewModelLocator().Main.Project;
                project.scriptList.Add((ScriptModel)File);
                RaisePropertyChanged(() => project.scriptList);
            }
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
        }

        private void DeleteElement(object item)
        {
            ProjectModel project = new ViewModelLocator().Main.Project;
            if (item is FocusGridModel)
            {
                project.fociList.Remove((FocusGridModel)item);
                RaisePropertyChanged(() => project.fociList);
            }
            else if (item is LocalisationModel)
            {
                project.localisationList.Remove((LocalisationModel)item);
                RaisePropertyChanged(() => project.localisationList);
            }
            else if (item is EventTabModel)
            {
                project.eventList.Remove((EventTabModel)item);
                RaisePropertyChanged(() => project.eventList);
            }
            else if (item is ScriptModel)
            {
                project.scriptList.Remove((ScriptModel)item);
                RaisePropertyChanged(() => project.scriptList);
            }
        }

        private void EditElement(object obj)
        {
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("EditAnyFile", false);
            FileManager dialog = new FileManager(ModeType.Edit);
            if (obj is FocusGridModel)
            {
                FocusGridModel item = (FocusGridModel)obj;
                new ViewModelLocator().FileManager.File = new FocusGridModel(item.VisibleName)
                {
                    TAG = item.TAG,
                    AdditionnalMods = item.AdditionnalMods
                };
                dialog.ShowDialog();
                if (new ViewModelLocator().FileManager.File == null) return;
                FocusGridModel newItem = new ViewModelLocator().FileManager.File as FocusGridModel;
                if (newItem == null) return;
                item.VisibleName = newItem.VisibleName;
                item.TAG = newItem.TAG;
                item.AdditionnalMods = newItem.AdditionnalMods;
            }
            else if (obj is LocalisationModel)
            {
                LocalisationModel item = (LocalisationModel) obj;
                new ViewModelLocator().FileManager.File = new LocalisationModel(item.VisibleName)
                {
                    LanguageName = item.LanguageName
                };
                dialog.ShowDialog();
                if (new ViewModelLocator().FileManager.File == null) return;
                LocalisationModel newItem = new ViewModelLocator().FileManager.File as LocalisationModel;
                if (newItem == null) return;
                item.VisibleName = newItem.VisibleName;
                item.LanguageName = newItem.LanguageName;
            }
            else if (obj is EventTabModel)
            {
                EventTabModel item = (EventTabModel) obj;
                new ViewModelLocator().FileManager.File = new EventTabModel(item.VisibleName)
                {
                    EventNamespace = item.EventNamespace
                };
                dialog.ShowDialog();
                if (new ViewModelLocator().FileManager.File == null) return;
                EventTabModel newItem = (new ViewModelLocator()).FileManager.File as EventTabModel;
                if (newItem == null) return;
                item.VisibleName = newItem.VisibleName;
                item.EventNamespace = newItem.EventNamespace;
            }
            else if (obj is ScriptModel)
            {
                ScriptModel item = (ScriptModel) obj;
                new ViewModelLocator().FileManager.File = new ScriptModel(item.VisibleName);
                dialog.ShowDialog();
                if (new ViewModelLocator().FileManager.File == null) return;
                ScriptModel newItem = new ViewModelLocator().FileManager.File as ScriptModel;
                if (newItem != null) item.VisibleName = newItem.VisibleName;
            }
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
        }

        private void CopyElement(object obj)
        {
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("CopyAnyFile", false);
            FileManager dialog = new FileManager(ModeType.Edit);
            if (obj is FocusGridModel)
            {
                FocusGridModel item = new FocusGridModel((FocusGridModel) obj);
                item.VisibleName += "_copy";
                Project.fociList.Add(item);
                RaisePropertyChanged(() => Project.fociList);
            }
            else if (obj is LocalisationModel)
            {
                LocalisationModel item = new LocalisationModel((LocalisationModel)obj);
                item.VisibleName += "_copy";
                Project.localisationList.Add(item);
                RaisePropertyChanged(() => Project.localisationList);
            }
            else if (obj is EventTabModel)
            {
                EventTabModel item = new EventTabModel((EventTabModel)obj);
                item.VisibleName += "_copy";
                Project.eventList.Add(item);
                RaisePropertyChanged(() => Project.eventList);
            }
            else if (obj is ScriptModel)
            {
                ScriptModel item = new ScriptModel((ScriptModel)obj);
                item.VisibleName += "_copy";
                Project.scriptList.Add(item);
                RaisePropertyChanged(() => Project.scriptList);
            }
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
        }

        private static void OpenFile(ObservableObject item)
        {
            if (item is FocusGridModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, new ViewModelLocator().Main, "OpenFocusTree"));
            }
            else if (item is LocalisationModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, new ViewModelLocator().Main, "OpenLocalisation"));
            }
            else if (item is EventTabModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, new ViewModelLocator().Main, "OpenEventList"));
            }
            else if (item is ScriptModel)
            {
                Messenger.Default.Send(new NotificationMessage(
                    item, new ViewModelLocator().Main, "OpenScriptList"));
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

        public void CopyFile()
        {
            CopyElement(SelectedItem);
        }

        public bool CanExecuteOnFile()
        {
            return SelectedItem != null;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            if (msg.Notification == "SendDeleteItemSignal")
            {
                DeleteElement(msg.Sender);
            }
            if (msg.Notification == "SendEditItemSignal")
            {
                EditElement(msg.Sender);
            }
            if (msg.Notification == "SendCopyItemSignal")
            {
                CopyElement(msg.Sender);
            }
            if (msg.Notification == "RefreshProjectViewer")
            {
                RaisePropertyChanged(() => Project);
                RaisePropertyChanged(() => Project.fociList);
                RaisePropertyChanged(() => Project.localisationList);
                RaisePropertyChanged(() => Project.eventList);
                RaisePropertyChanged(() => Project.scriptList);
            }
            if (msg.Target == this)
            {
                //Resend to the tutorial View model if this was the target
                Messenger.Default.Send(new NotificationMessage(msg.Sender,
                new ViewModelLocator().Tutorial, msg.Notification));
            }
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return new ViewModelLocator().Main;
        }

        #endregion
    }
}