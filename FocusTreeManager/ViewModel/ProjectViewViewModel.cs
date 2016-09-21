using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Messaging;

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
        public string SelectedFile { get; private set; }

        public ObservableCollection<FociGridContainer> fociContainerList
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.fociContainerList;
            }
        }

        public ObservableCollection<LocalisationContainer> localisationList
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.localisationList;
            }
        }

        public RelayCommand<string> AddFileCommand { get; private set; }

        public RelayCommand<string> AddElementCommand { get; private set; }

        public RelayCommand<string> OpenFileTreeCommand { get; private set; }

        public RelayCommand<string> OpenFileLocaleCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProjectViewModel class.
        /// </summary>
        public ProjectViewViewModel()
        {
            //Commands
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddElementCommand = new RelayCommand<string>(AddElement);
            OpenFileTreeCommand = new RelayCommand<string>(OpenFocusTree);
            OpenFileLocaleCommand = new RelayCommand<string>(OpenLocalisation);
        }

        private void AddElement(string param)
        {
            switch(param)
            {
                case "FocusTree" :
                {
                    fociContainerList.Add(new FociGridContainer("FocusTree" + fociContainerList.Count().ToString()));
                    break;
                }
                case "Localisation":
                {
                    localisationList.Add(new LocalisationContainer("Localisation" + localisationList.Count().ToString()));
                    break;
                }
            }
        }

        private void AddFile(string param)
        {
            throw new NotImplementedException();
        }

        private void OpenFocusTree(string param)
        {
            SelectedFile = param;
            Messenger.Default.Send(new NotificationMessage(this, "OpenFocusTree"));
        }

        private void OpenLocalisation(string param)
        {
            SelectedFile = param;
            Messenger.Default.Send(new NotificationMessage(this, "OpenLocalisation"));
        }
    }
}