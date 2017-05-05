using FocusTreeManager.DataContract;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FocusTreeManager.Model.TabModels;
using FocusTreeManager.Helper;

namespace FocusTreeManager.ViewModel
{
    public class MainViewModel : ViewModelBase, ISupportsUndo
    {
        private IDialogCoordinator coordinator;

        private bool isProjectExist = false;

        public bool IsProjectExist
        {
            get
            {
                return isProjectExist;
            }
            set
            {
                isProjectExist = value;
                RaisePropertyChanged(() => IsProjectExist);
            }
        }

        private ProjectModel project;

        public ProjectModel Project
        {
            get
            {
                return project;
            }
            set
            {
                project = value;
                RaisePropertyChanged(() => Project);
            }
        }

        public ObservableCollection<ObservableObject> TabsModelList { get; private set; }

        private ObservableObject selectedTab;

        public ObservableObject SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if (value == selectedTab) return;
                selectedTab = value;
                if (selectedTab is FocusGridModel)
                {
                    ((FocusGridModel)selectedTab).RedrawGrid();
                }
            }
        }

        public RelayCommand NewProjectCommand { get; private set; }

        public RelayCommand LoadProjectCommand { get; private set; }

        public RelayCommand SaveProjectCommand { get; private set; }

        public RelayCommand SaveProjectAsCommand { get; private set; }

        public RelayCommand EditProjectCommand { get; private set; }

        public RelayCommand ExportProjectCommand { get; private set; }

        public RelayCommand UndoCommand { get; private set; }

        public RelayCommand RedoCommand { get; private set; }

        public MainViewModel()
        {
            coordinator = DialogCoordinator.Instance;
            TabsModelList = new ObservableCollection<ObservableObject>();
            //Commands
            NewProjectCommand = new RelayCommand(() => checkBeforeContinuing("New"));
            LoadProjectCommand = new RelayCommand(() => checkBeforeContinuing("Load"));
            SaveProjectCommand = new RelayCommand(saveProject);
            SaveProjectAsCommand = new RelayCommand(saveProjectAs);
            EditProjectCommand = new RelayCommand(editProject);
            ExportProjectCommand = new RelayCommand(ExportProject);
            UndoCommand = new RelayCommand(UndoExecute, UndoCanExecute);
            RedoCommand = new RelayCommand(RedoExecute, RedoCanExecute);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        async private void checkBeforeContinuing(string command)
        {
            if (isProjectExist)
            {
                string Title = LocalizationHelper.getValueForKey("Exit_Confirm_Title");
                string Message = LocalizationHelper.getValueForKey("Delete_Confirm");
                MetroDialogSettings settings = new MetroDialogSettings();
                settings.AffirmativeButtonText = LocalizationHelper.getValueForKey("Command_Save");
                settings.NegativeButtonText = LocalizationHelper.getValueForKey("Command_Cancel");
                settings.FirstAuxiliaryButtonText = LocalizationHelper.getValueForKey("Command_Continue");
                MessageDialogResult Result = await coordinator.ShowMessageAsync(this, Title, Message,
                                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
                if (Result == MessageDialogResult.Affirmative)
                {
                    saveProject();
                    if (command == "New")
                    {
                        newProject();
                    }
                    if (command == "Load")
                    {
                        loadProject();
                    }
                }
                else if (Result == MessageDialogResult.FirstAuxiliary)
                {
                    if (command == "New")
                    {
                        newProject();
                    }
                    if (command == "Load")
                    {
                        loadProject();
                    }
                }
            }
            else
            {
                if (command == "New")
                {
                    newProject();
                }
                if (command == "Load")
                {
                    loadProject();
                }
            }
        }

        async private void newProject()
        {
            var Vm = (new ViewModelLocator()).ProjectEditor;
            ProjectEditor dialog = new ProjectEditor();
            Vm.Project = new ProjectModel();
            dialog.ShowDialog();
            if (Vm.Project == null) return;
            //Check if the files already exists, if yes, show a message
            if (File.Exists(Vm.Project.Filename))
            {
                MessageDialogResult Result = await ShowProjectExistDialog();
                if (Result != MessageDialogResult.Affirmative)
                {
                    //If the user does not want the file to be overwritten
                    return;
                }
            }
            Project = Vm.Project;
            IsProjectExist = true;
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                        "RefreshProjectViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            RaisePropertyChanged(() => IsProjectExist);
            TabsModelList = new ObservableCollection<ObservableObject>();
            RaisePropertyChanged(() => TabsModelList);
            UndoService.Current[this].Clear();
            try
            {
                DataHolder.Instance.SaveContract(Project);
            }
            catch (Exception)
            {
                string Title = LocalizationHelper.getValueForKey("Application_Error");
                string Message = LocalizationHelper.getValueForKey("Application_Error_Saving");
                coordinator.ShowMessageAsync(this, Title, Message);
            }
        }

        async private Task<MessageDialogResult> ShowProjectExistDialog()
        {
            string Title = LocalizationHelper.getValueForKey("Application_Project_Exists_Header");
            string Message = LocalizationHelper.getValueForKey("Application_Project_Exists");
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = LocalizationHelper.getValueForKey("Command_Yes");
            settings.NegativeButtonText = LocalizationHelper.getValueForKey("Command_No");
            return await coordinator.ShowMessageAsync(this, Title, Message,
                MessageDialogStyle.AffirmativeAndNegative, settings);
        }

        public void editProject()
        {
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("EditProject", false);
            var Vm = (new ViewModelLocator()).ProjectEditor;
            ProjectEditor dialog = new ProjectEditor();
            Vm.Project = Project;
            dialog.ShowDialog();
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
        }

        private void loadProject()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.Title = LocalizationHelper.getValueForKey("Project_Load");
                dialog.InitialDirectory = "C:";
                dialog.AddToMostRecentlyUsedList = false;
                dialog.AllowNonFileSystemItems = false;
                dialog.DefaultDirectory = "C:";
                dialog.EnsureFileExists = true;
                dialog.EnsurePathExists = true;
                dialog.EnsureReadOnly = false;
                dialog.EnsureValidNames = true;
                dialog.Filters.Add(new CommonFileDialogFilter("Project", "*.xh4prj"));
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    LoadProject(dialog.FileName);
                    Mouse.OverrideCursor = null;
                }
            }
            catch (Exception)
            {
                string Title = LocalizationHelper.getValueForKey("Application_Error");
                string Message = LocalizationHelper.getValueForKey("Application_Error_Loading");
                coordinator.ShowMessageAsync(this, Title, Message);
            }
        }

        public void LoadProject(string path)
        {
            if (Path.GetExtension(path) == ".h4prj")
            {
                string Title = LocalizationHelper.getValueForKey("Application_Loading");
                string Message = LocalizationHelper.getValueForKey("Application_Legacy_Loading");
                coordinator.ShowMessageAsync(this, Title, Message);
            }
            if (!DataHolder.LoadContract(path))
            {
                string Title = LocalizationHelper.getValueForKey("Application_Error");
                string Message = LocalizationHelper.getValueForKey("Application_Error_Transfer");
                coordinator.ShowMessageAsync(this, Title, Message);
                return;
            }
            project = ProjectModel.createFromDataContract(DataHolder.Instance.Project);
            AsyncImageLoader.AsyncImageLoader.Worker.RefreshFromMods();
            project.Filename = path;
            RaisePropertyChanged(() => IsProjectExist);
            TabsModelList = new ObservableCollection<ObservableObject>();
            RaisePropertyChanged(() => TabsModelList);
            IsProjectExist = true;
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                "RefreshProjectViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            UndoService.Current[this].Clear();
        }

        private void saveProject()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (isProjectExist && !string.IsNullOrEmpty(DataHolder.Instance.Project.filename))
            {
                try
                {
                    DataHolder.Instance.SaveContract(Project);
                }
                catch (Exception)
                {
                    string Title = LocalizationHelper.getValueForKey("Application_Error");
                    string Message = LocalizationHelper.getValueForKey("Application_Error_Saving");
                    coordinator.ShowMessageAsync(this, Title, Message);
                }
            }
            else if (isProjectExist)
            {
                saveProjectAs();
            }
            Mouse.OverrideCursor = null;
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
        }

        private void saveProjectAs()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.Title = LocalizationHelper.getValueForKey("Project_Save");
                dialog.InitialDirectory = string.IsNullOrEmpty(DataHolder.Instance.Project.filename) ? "C:" 
                    : DataHolder.Instance.Project.filename;
                dialog.AddToMostRecentlyUsedList = false;
                dialog.AllowNonFileSystemItems = false;
                dialog.DefaultDirectory = "C:";
                dialog.EnsureFileExists = false;
                dialog.EnsurePathExists = true;
                dialog.EnsureReadOnly = false;
                dialog.EnsureValidNames = true;
                dialog.Filters.Add(new CommonFileDialogFilter("Project", "*.xh4prj"));
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Project.Filename = Path.Combine(Path.GetDirectoryName(dialog.FileName),
                        Path.GetFileNameWithoutExtension(dialog.FileName)) + ".xh4prj";
                    DataHolder.Instance.SaveContract(Project);
                }
                Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            }
            catch (Exception)
            {
                string Title = LocalizationHelper.getValueForKey("Application_Error");
                string Message = LocalizationHelper.getValueForKey("Application_Error_Saving");
                coordinator.ShowMessageAsync(this, Title, Message);
            }
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            if (msg.Notification == "OpenFocusTree")
            {
                FocusGridModel container = msg.Sender as FocusGridModel;
                if (TabsModelList.Contains(container)) return;
                CheckForChanges(container);
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                LocalisationModel container = msg.Sender as LocalisationModel;
                if (TabsModelList.Contains(container)) return;
                CheckForChanges(container);
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenEventList")
            {
                EventTabModel container = msg.Sender as EventTabModel;
                if (TabsModelList.Contains(container)) return;
                CheckForChanges(container);
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenScriptList")
            {
                ScriptModel container = msg.Sender as ScriptModel;
                if (TabsModelList.Contains(container)) return;
                CheckForChanges(container);
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "SaveProject")
            {
                saveProject();
            }
            if (msg.Notification == "RefreshProjectViewer")
            {
                TabsModelList.Clear();
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "SendDeleteItemSignal")
            {
                ObservableObject Model = null;
                if (msg.Sender is FocusGridModel)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is FocusGridModel && 
                            ((FocusGridModel)m).UniqueID == ((FocusGridModel)msg.Sender).UniqueID);
                }
                else if (msg.Sender is LocalisationModel)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is LocalisationModel &&
                            ((LocalisationModel)m).UniqueID == ((LocalisationModel)msg.Sender).UniqueID);
                }
                else if (msg.Sender is EventTabModel)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is EventTabModel &&
                            ((EventTabModel)m).UniqueID == ((EventTabModel)msg.Sender).UniqueID);
                }
                else if (msg.Sender is ScriptModel)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is ScriptModel &&
                            ((ScriptModel)m).UniqueID == ((ScriptModel)msg.Sender).UniqueID);
                }
                TabsModelList.Remove(Model);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Target == this)
            {
                //Resend to the tutorial View model if this was the target
                Messenger.Default.Send(new NotificationMessage(msg.Sender,
                new ViewModelLocator().Tutorial, msg.Notification));
            }
        }

        private void CheckForChanges(ObservableObject container)
        {
            if (container is FocusGridModel)
            {
                ((FocusGridModel)container).CheckForChanges();
            }
            else if (container is LocalisationModel)
            {
                ((LocalisationModel)container).CheckForChanges();
            }
            else if (container is EventTabModel)
            {
                ((EventTabModel)container).CheckForChanges();
            }
            else if (container is ScriptModel)
            {
                ((ScriptModel)container).CheckForChanges();
            }
        }

        async public Task<MessageDialogResult> ShowFileChangedDialog()
        {
            string Title = LocalizationHelper.getValueForKey("Application_File_Changed_Header");
            string Message = LocalizationHelper.getValueForKey("Application_File_Changed");
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = LocalizationHelper.getValueForKey("Command_Yes");
            settings.NegativeButtonText = LocalizationHelper.getValueForKey("Command_No");
            return await coordinator.ShowMessageAsync(this, Title, Message, 
                MessageDialogStyle.AffirmativeAndNegative, settings);
        }

        public void ExportProject()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = LocalizationHelper.getValueForKey("Project_Export");
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = "C:";
            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = "C:";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DataHolder.Instance.Project.UpdateDataContract(Project);
                DataHolder.Instance.Project.ExportProject(dialog.FileName);
            }
        }

        #region Undo/Redo

        private ICommand windowLoadedCommand;

        public ICommand WindowLoadedCommand
        {
            get
            {
                return windowLoadedCommand ?? 
                    (windowLoadedCommand = new RelayCommand(OnWindowLoaded));
            }
        }

        private void OnWindowLoaded()
        {
            var root = UndoService.Current[this];
            root.UndoStackChanged += new EventHandler(OnUndoStackChanged);
            root.RedoStackChanged += new EventHandler(OnRedoStackChanged);
        }

        // Refresh the UI when the undo stack changes.
        void OnUndoStackChanged(object sender, EventArgs e)
        {
            UndoCommand.RaiseCanExecuteChanged();
        }

        // Refresh the UI when the redo stack changes.
        void OnRedoStackChanged(object sender, EventArgs e)
        {
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void RedoExecute()
        {
            UndoService.Current[this].Redo();
        }

        private bool RedoCanExecute()
        {
            return UndoService.Current[this].CanRedo;
        }

        private void UndoExecute()
        {
            var undoRoot = UndoService.Current[this];
            undoRoot.Undo();
            //Refresh all focus grids if needed
            if (selectedTab is FocusGridModel)
            {
                ((FocusGridModel)selectedTab).RedrawGrid();
            }
        }

        private bool UndoCanExecute()
        {
            // Tell the UI whether Undo is available.
            return UndoService.Current[this].CanUndo;
        }

        public object GetUndoRoot()
        {
            return this;
        }

        #endregion
    }
}