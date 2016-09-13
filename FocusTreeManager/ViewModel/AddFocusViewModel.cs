using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddFocusViewModel : ViewModelBase
    {
        private Focus addedFocus;

        public Focus AddedFocus
        {
            get
            {
                return addedFocus;
            }
            set
            {
                addedFocus = value;
                RaisePropertyChanged("AddedFocus");
            }
        }

        public RelayCommand AddFocusCommand { get; private set; }

        public RelayCommand ChangeImageCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AddFocusViewModel class.
        /// </summary>
        public AddFocusViewModel()
        {
            AddFocusCommand = new RelayCommand(AddFocus);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public void AddFocus()
        {
            Messenger.Default.Send(new NotificationMessage(this, "HideAddFocus"));
        }

        public void ChangeImage()
        {
            Messenger.Default.Send(new NotificationMessage(this, "ShowChangeImage"));
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ShowAddFocus")
            {
                AddedFocus = new Focus();
                AddedFocus.setDefaults();
                RaisePropertyChanged("AddedFocus");
            }
            if (msg.Notification == "HideChangeImage")
            {
                if ((string)System.Windows.Application.Current.Properties["Mode"] == "Add")
                { 
                    ChangeImageViewModel viewModel = (ChangeImageViewModel)msg.Sender;
                    addedFocus.Image = viewModel.FocusImage;
                }
            }
        }
    }
}