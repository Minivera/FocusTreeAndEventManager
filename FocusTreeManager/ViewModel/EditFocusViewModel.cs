using FocusTreeManager.CodeStructures;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
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
        private Model.Focus focus;

        public Model.Focus Focus
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

        public RelayCommand EditScriptCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EditFocusViewModel class.
        /// </summary>
        public EditFocusViewModel()
        {
            FocusCommand = new RelayCommand(EditFocus);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            EditScriptCommand = new RelayCommand(EditScript);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public EditFocusViewModel SetupFlyout()
        {
            RaisePropertyChanged("Focus");
            return this;
        }

        public void EditFocus()
        {
            Messenger.Default.Send(new NotificationMessage(this, "HideEditFocus"));
        }

        public void ChangeImage()
        {
            ChangeImage view = new ChangeImage();
            (new ViewModelLocator()).ChangeImage.LoadImages("Focus", Focus.Icon);
            view.ShowDialog();
            Focus.Image = (new ViewModelLocator()).ChangeImage.FocusImage;
        }

        public void EditScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(Focus.InternalScript,
                ScripterControlsViewModel.ScripterType.FocusTree);
            dialog.ShowDialog();
            focus.InternalScript = ViewModel.ManagedScript;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Target != null && msg.Target != this)
            {
                //Message not intended for here
                return;
            }
        }
    }
}