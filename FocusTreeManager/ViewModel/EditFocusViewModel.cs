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
    public class EditFocusViewModel : ViewModelBase
    {
        private Focus focus;

        public Focus Focus
        {
            get
            {
                return focus;
            }
            set
            {
                focus = value;
                RaisePropertyChanged("Focus");
            }
        }

        public RelayCommand FocusCommand { get; private set; }

        public RelayCommand ChangeImageCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EditFocusViewModel class.
        /// </summary>
        public EditFocusViewModel()
        {
            FocusCommand = new RelayCommand(EditFocus);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public void EditFocus()
        {
            Messenger.Default.Send(new NotificationMessage(this, "HideEditFocus"));
        }

        public void ChangeImage()
        {
            Messenger.Default.Send(new NotificationMessage(this, "ShowChangeImage"));
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "HideChangeImage")
            {
                if ((string)System.Windows.Application.Current.Properties["Mode"] == "Edit")
                {
                    ChangeImageViewModel viewModel = (ChangeImageViewModel)msg.Sender;
                    Focus.Image = viewModel.FocusImage;
                }
            }
        }
    }
}