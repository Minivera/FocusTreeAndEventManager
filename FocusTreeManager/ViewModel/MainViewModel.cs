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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FocusTreeManager.ViewModel
{
    public class MainViewModel : ViewModelBase, ISupportsUndo
    {
        private IDialogCoordinator coordinator;

        private string statusText;

        public string StatusText
        {
            get
            {
                return statusText;
            }
            set
            {
                statusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

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
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                string Title = resourceLocalization["Exit_Confirm_Title"] as string;
                string Message = resourceLocalization["Delete_Confirm"] as string;
                MetroDialogSettings settings = new MetroDialogSettings();
                settings.AffirmativeButtonText = resourceLocalization["Command_Save"] as string;
                settings.NegativeButtonText = resourceLocalization["Command_Cancel"] as string;
                settings.FirstAuxiliaryButtonText = resourceLocalization["Command_Continue"] as string;
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

        private void newProject()
        {
            var Vm = (new ViewModelLocator()).ProjectEditor;
            ProjectEditor dialog = new ProjectEditor();
            Vm.Project = new ProjectModel();
            dialog.ShowDialog();
            if (Vm.Project != null)
            {
                Project = Vm.Project;
                IsProjectExist = true;
                Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                            "RefreshProjectViewer"));
                Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
                Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
                RaisePropertyChanged("isProjectExist");
                TabsModelList = new ObservableCollection<ObservableObject>();
                RaisePropertyChanged("TabsModelList");
                UndoService.Current[this].Clear();
            }
        }

        public void editProject()
        {
            var Vm = (new ViewModelLocator()).ProjectEditor;
            ProjectEditor dialog = new ProjectEditor();
            Vm.Project = Project;
            dialog.ShowDialog();
        }

        private void loadProject()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                dialog.Title = resourceLocalization["Project_Load"] as string;
                dialog.InitialDirectory = "C:";
                dialog.AddToMostRecentlyUsedList = false;
                dialog.AllowNonFileSystemItems = false;
                dialog.DefaultDirectory = "C:";
                dialog.EnsureFileExists = true;
                dialog.EnsurePathExists = true;
                dialog.EnsureReadOnly = false;
                dialog.EnsureValidNames = true;
                dialog.Filters.Add(new CommonFileDialogFilter("Project", "*.xh4prj"));
                dialog.Filters.Add(new CommonFileDialogFilter("Beta project", "*.h4prj"));
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    LoadProject(dialog.FileName);
                }
            }
            catch (Exception)
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                string Title = resourceLocalization["Application_Error"] as string;
                string Message = resourceLocalization["Application_Error_Loading"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
            }
        }

        public void LoadProject(string path)
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            if (Path.GetExtension(path) == ".h4prj")
            {
                string Title = resourceLocalization["Application_Loading"] as string;
                string Message = resourceLocalization["Application_Legacy_Loading"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
            }
            if (!DataHolder.LoadContract(path))
            {
                string Title = resourceLocalization["Application_Error"] as string;
                string Message = resourceLocalization["Application_Error_Transfer"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
                return;
            }
            project = ProjectModel.createFromDataContract(DataHolder.Instance.Project);
            project.Filename = path;
            RaisePropertyChanged("isProjectExist");
            TabsModelList = new ObservableCollection<ObservableObject>();
            RaisePropertyChanged("TabsModelList");
            IsProjectExist = true;
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                "RefreshProjectViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            UndoService.Current[this].Clear();
        }

        private void saveProject()
        {
            if (isProjectExist && !string.IsNullOrEmpty(DataHolder.Instance.Project.filename))
            {
                try
                {
                    DataHolder.Instance.SaveContract(Project);
                }
                catch (Exception)
                {
                    ResourceDictionary resourceLocalization = new ResourceDictionary();
                    resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                    string Title = resourceLocalization["Application_Error"] as string;
                    string Message = resourceLocalization["Application_Error_Saving"] as string;
                    coordinator.ShowMessageAsync(this, Title, Message);
                }
            }
            else if (isProjectExist)
            {
                saveProjectAs();
            }
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
        }

        private void saveProjectAs()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                dialog.Title = resourceLocalization["Project_Save"] as string;
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
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                string Title = resourceLocalization["Application_Error"] as string;
                string Message = resourceLocalization["Application_Error_Saving"] as string;
                coordinator.ShowMessageAsync(this, Title, Message);
            }
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "OpenFocusTree")
            {
                FocusGridModel container = msg.Sender as FocusGridModel;
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                LocalisationModel container = msg.Sender as LocalisationModel;
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenEventList")
            {
                EventTabModel container = msg.Sender as EventTabModel;
                TabsModelList.Add(container);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenScriptList")
            {
                ScriptModel container = msg.Sender as ScriptModel;
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
        }

        public void ExportProject()
        {
            var dialog = new CommonOpenFileDialog();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            dialog.Title = resourceLocalization["Project_Export"] as string;
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
            foreach (FocusGridModel model in Project.fociList.Where(m => m.isShown))
            {
                model.RedrawGrid();
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