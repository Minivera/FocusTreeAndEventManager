using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FocusTreeManager.ViewModel
{
    public class ProjectEditorViewModel : ViewModelBase
    {
        private ProjectModel project;

        public ProjectModel Project
        {
            get
            {
                return project;
            }
            set
            {
                if (value == project)
                {
                    return;
                }
                project = value;
                RaisePropertyChanged(() => Project);
            }
        }

        public RelayCommand AcceptCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public ProjectEditorViewModel()
        {
            AcceptCommand = new RelayCommand(Accept);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Accept()
        {
            Close();
        }

        public void Cancel()
        {
            Project = null;
            Close();
        }

        private void Close()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }
    }
}