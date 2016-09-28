using Dragablz;
using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        private string commandToContinue;

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

        public MainViewModel()
        {
            TabsModelList = new ObservableCollection<ObservableObject>();
            //Commands
            NewProjectCommand = new RelayCommand(() => checkBeforeContinuing("New"));
            LoadProjectCommand = new RelayCommand(() => checkBeforeContinuing("Load"));
            SaveProjectCommand = new RelayCommand(saveProject);
            SaveProjectAsCommand = new RelayCommand(saveProjectAs);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void checkBeforeContinuing(string command)
        {
            if (project != null)
            {
                commandToContinue = command;
                Messenger.Default.Send(new NotificationMessage(this, "ConfirmBeforeContinue"));
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
            Messenger.Default.Send(new NotificationMessage(this, "HideProjectControl"));
            RaisePropertyChanged("isProjectExist");
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
                Messenger.Default.Send(new NotificationMessage(this, "ErrorLoadingProject"));
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
                    Messenger.Default.Send(new NotificationMessage(this, "ErrorSavingProject"));
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
                Messenger.Default.Send(new NotificationMessage(this, "ErrorSavingProject"));
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
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;
                FociGridContainer container = model.SelectedFile as FociGridContainer;
                if (TabsModelList.Where((t) => t is FocusGridModel && 
                        ((FocusGridModel)t).Filename == container.ContainerID).Any())
                {
                    return;
                }
                FocusGridModel newModel = new FocusGridModel(container.IdentifierID);
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;
                LocalisationContainer container = model.SelectedFile as LocalisationContainer;
                if (TabsModelList.Where((t) => t is LocalisationModel && 
                            ((LocalisationModel)t).Filename == container.ContainerID).Any())
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
            if (msg.Notification == "ContinueCommand")
            {
                if (commandToContinue == "New")
                {
                    newProject();
                }
                if (commandToContinue == "Load")
                {
                    loadProject();
                }
            }
            if (msg.Notification == "RefreshProjectViewer")
            {
                TabsModelList.Clear();
                RaisePropertyChanged("TabsModelList");
            }
        }
    }
}