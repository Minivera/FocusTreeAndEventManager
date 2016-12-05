using FocusTreeManager.DataContract;
using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using FocusTreeManager.Parsers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        const string FOCUS_TREE_PATH = @"\common\national_focus\";

        const string LOCALISATION_PATH = @"\localisation\";

        const string EVENTS_PATH = @"\events\";

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

        public ObservableCollection<ObservableObject> TabsModelList { get; private set; }
        
        public RelayCommand NewProjectCommand { get; private set; }

        public RelayCommand LoadProjectCommand { get; private set; }

        public RelayCommand SaveProjectCommand { get; private set; }

        public RelayCommand SaveProjectAsCommand { get; private set; }

        public RelayCommand ExportProjectCommand { get; private set; }

        public MainViewModel()
        {
            coordinator = DialogCoordinator.Instance;
            TabsModelList = new ObservableCollection<ObservableObject>();
            //Commands
            NewProjectCommand = new RelayCommand(() => checkBeforeContinuing("New"));
            LoadProjectCommand = new RelayCommand(() => checkBeforeContinuing("Load"));
            SaveProjectCommand = new RelayCommand(saveProject);
            SaveProjectAsCommand = new RelayCommand(saveProjectAs);
            ExportProjectCommand = new RelayCommand(ExportProject);
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
            Project.ResetInstance();
            IsProjectExist = true;
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                        "RefreshProjectViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            RaisePropertyChanged("isProjectExist");
            TabsModelList = new ObservableCollection<ObservableObject>();
            RaisePropertyChanged("TabsModelList");
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
                    if (Path.GetExtension(dialog.FileName) == ".h4prj")
                    {
                        resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                        string Title = resourceLocalization["Application_Loading"] as string;
                        string Message = resourceLocalization["Application_Legacy_Loading"] as string;
                        coordinator.ShowMessageAsync(this, Title, Message);
                    }
                    Project returnVal = SerializationHelper.Deserialize(dialog.FileName);
                    if (returnVal == null)
                    {
                        resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                        string Title = resourceLocalization["Application_Error"] as string;
                        string Message = resourceLocalization["Application_Error_Transfer"] as string;
                        coordinator.ShowMessageAsync(this, Title, Message);
                        return;
                    }
                    else
                    {
                        Project.SetInstance(returnVal);
                    }
                    Project.Instance.filename = dialog.FileName;
                    RaisePropertyChanged("isProjectExist");
                    TabsModelList = new ObservableCollection<ObservableObject>();
                    RaisePropertyChanged("TabsModelList");
                    IsProjectExist = true;
                    Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView,
                        "RefreshProjectViewer"));
                    Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
                    Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
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

        private void saveProject()
        {
            if (isProjectExist && !string.IsNullOrEmpty(Project.Instance.filename))
            {
                try
                {
                    SerializationHelper.Serialize(Project.Instance.filename, Project.Instance);
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
                dialog.InitialDirectory = string.IsNullOrEmpty(Project.Instance.filename) ? "C:" 
                    : Project.Instance.filename;
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
                    Project.Instance.filename = Path.Combine(Path.GetDirectoryName(dialog.FileName),
                        Path.GetFileNameWithoutExtension(dialog.FileName)) + ".xh4prj";
                    SerializationHelper.Serialize(Project.Instance.filename, Project.Instance);
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
                FociGridContainer container = msg.Sender as FociGridContainer;
                if (TabsModelList.Where((t) => t is FocusGridModel && 
                        ((FocusGridModel)t).UniqueID == container.IdentifierID).Any())
                {
                    return;
                }
                FocusGridModel newModel = new FocusGridModel(container.IdentifierID);
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                LocalisationContainer container = msg.Sender as LocalisationContainer;
                if (TabsModelList.Where((t) => t is LocalisationModel && 
                            ((LocalisationModel)t).UniqueID == container.IdentifierID).Any())
                {
                    return;
                }
                LocalisationModel newModel = new LocalisationModel(container.IdentifierID);
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenEventList")
            {
                EventContainer container = msg.Sender as EventContainer;
                if (TabsModelList.Where((t) => t is EventModel &&
                            ((EventTabModel)t).UniqueID == container.IdentifierID).Any())
                {
                    return;
                }
                EventTabModel newModel = new EventTabModel(container.IdentifierID);
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenScriptList")
            {
                ScriptContainer container = msg.Sender as ScriptContainer;
                if (TabsModelList.Where((t) => t is ScriptModel &&
                            ((ScriptModel)t).UniqueID == container.IdentifierID).Any())
                {
                    return;
                }
                ScriptModel newModel = new ScriptModel(container.IdentifierID);
                TabsModelList.Add(newModel);
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
                if (msg.Sender is FociGridContainer)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is FocusGridModel && 
                            ((FocusGridModel)m).UniqueID == ((FociGridContainer)msg.Sender).IdentifierID);
                }
                else if (msg.Sender is LocalisationContainer)
                {
                    Model = TabsModelList.FirstOrDefault((m) => m is LocalisationModel &&
                            ((LocalisationModel)m).UniqueID == ((LocalisationContainer)msg.Sender).IdentifierID);
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
                string path = dialog.FileName + FOCUS_TREE_PATH;
                Directory.CreateDirectory(path);
                //For each parsed focus trees
                foreach (KeyValuePair<string, string> item in 
                    FocusTreeParser.ParseAllTrees(Project.Instance.fociContainerList))
                {
                    using (TextWriter tw = new StreamWriter(path + item.Key + ".txt"))
                    {
                        tw.Write(item.Value);
                    }
                }
                path = dialog.FileName + LOCALISATION_PATH;
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                //For each parsed localisation files
                foreach (KeyValuePair<string, string> item in
                    LocalisationParser.ParseEverything(Project.Instance.localisationList))
                {
                    using (TextWriter tw = new StreamWriter(path + item.Key + ".yaml"))
                    {
                        tw.Write(item.Value);
                    }
                }
                path = dialog.FileName + EVENTS_PATH;
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                //For each parsed event file
                foreach (KeyValuePair<string, string> item in
                    EventParser.ParseAllEvents(Project.Instance.eventList))
                {
                    using (TextWriter tw = new StreamWriter(path + item.Key + ".txt"))
                    {
                        tw.Write(item.Value);
                    }
                }
                //For each parsed script file
                foreach (KeyValuePair<string, string> item in
                    ScriptParser.ParseEverything(Project.Instance.scriptList))
                {
                    using (TextWriter tw = new StreamWriter(dialog.FileName + "\\" + item.Key + ".txt"))
                    {
                        tw.Write(item.Value);
                    }
                }
            }
        }
    }
}