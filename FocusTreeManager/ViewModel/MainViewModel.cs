using Dragablz;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using FocusTreeManager.Parsers;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IDialogCoordinator coordinator;

        private Project project;

        public Project Project
        {
            get
            {
                return project;
            }
            set
            {
                project = value;
                RaisePropertyChanged("Project");
            }
        }

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

        public bool isProjectExist
        {
            get
            {
                return project != null;
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
            if (project != null)
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
            Project = new Project();
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
                dialog.DefaultExtension = "h4prj";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    using (var fs = File.OpenRead(dialog.FileName))
                    {
                        Project = Serializer.Deserialize<Project>(fs);
                    }
                    //Repair references
                    foreach (FociGridContainer container in Project.fociContainerList)
                    {
                        container.RepairInternalReferences();
                    }
                    Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView, 
                        "RefreshProjectViewer"));
                    Messenger.Default.Send(new NotificationMessage(this, "RefreshTabViewer"));
                    Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
                    RaisePropertyChanged("isProjectExist");
                    TabsModelList = new ObservableCollection<ObservableObject>();
                    RaisePropertyChanged("TabsModelList");
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
            if (project != null && !string.IsNullOrEmpty(project.filename))
            {
                try
                {
                    project.SaveToFile(project.filename);
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
            else if (project != null)
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
                dialog.InitialDirectory = string.IsNullOrEmpty(project.filename) ? "C:" : project.filename;
                dialog.AddToMostRecentlyUsedList = false;
                dialog.AllowNonFileSystemItems = false;
                dialog.DefaultDirectory = "C:";
                dialog.EnsureFileExists = false;
                dialog.EnsurePathExists = true;
                dialog.EnsureReadOnly = false;
                dialog.EnsureValidNames = true;
                dialog.DefaultExtension = "h4prj";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    project.SaveToFile(dialog.FileName);
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
            if (msg.Target != null && msg.Target != this)
            {
                //Message not itended for here
                return;
            }
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
                //For each parsed focus trees
                foreach (KeyValuePair<string, string> item in 
                    FocusTreeParser.ParseAllTrees(project.fociContainerList))
                {
                    using (TextWriter tw = new StreamWriter(dialog.FileName + "\\" + item.Key + ".txt"))
                    {
                        tw.Write(item.Value);
                    }
                }
                //For each parsed localisation files
                foreach (KeyValuePair<string, string> item in
                    LocalisationParser.ParseEverything(project.localisationList))
                {
                    using (TextWriter tw = new StreamWriter(dialog.FileName + "\\" + item.Key + ".yaml"))
                    {
                        tw.Write(item.Value);
                    }
                }
                //TODO: For each parsed event
            }
        }
    }
}