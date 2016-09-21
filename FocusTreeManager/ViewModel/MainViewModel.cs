using Dragablz;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
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

        public ObservableCollection<ObservableObject> TabsModelList { get; private set; }

        public MainViewModel()
        {
            Project = new Project();
            TabsModelList = new ObservableCollection<ObservableObject>();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "LoadProject")
            {
                try
                {
                    var dialog = new CommonOpenFileDialog();
                    ResourceDictionary resourceLocalization = new ResourceDictionary();
                    resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                    dialog.Title = resourceLocalization["Game_Path_Title"] as string;
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
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        project = (Project)formatter.Deserialize(stream);
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    //Call error
                }
            }
            if (msg.Notification == "OpenFocusTree")
            {
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;
                if (null != TabsModelList.SingleOrDefault((t) => t is FocusGridModel && 
                                         ((FocusGridModel)t).Filename == model.SelectedFile))
                {
                    return;
                }
                FocusGridModel newModel = new FocusGridModel()
                {
                    Filename = model.SelectedFile
                };
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;
                if (null != TabsModelList.SingleOrDefault((t) => t is LocalisationModel && 
                                         ((LocalisationModel)t).Filename == model.SelectedFile))
                {
                    return;
                }
                LocalisationModel newModel = new LocalisationModel(model.SelectedFile);
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
        }
    }
}