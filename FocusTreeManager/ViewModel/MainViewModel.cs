using Dragablz;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
                    //TODO: Getfilename
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                    project = (Project)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch (Exception ex)
                {
                    //Call error
                }
            }
            if (msg.Notification == "OpenFocusTree")
            {
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;
                if (null != TabsModelList.SingleOrDefault((t) => ((FocusGridModel)t).TreeId == model.SelectedFile))
                {
                    return;
                }
                FocusGridModel newModel = new FocusGridModel()
                {
                    TreeId = model.SelectedFile
                };
                TabsModelList.Add(newModel);
                RaisePropertyChanged("TabsModelList");
            }
            if (msg.Notification == "OpenLocalisation")
            {
                ProjectViewViewModel model = msg.Sender as ProjectViewViewModel;

            }
        }
    }
}