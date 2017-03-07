using FocusTreeManager.CodeStructures;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ErrorDawgViewModel : ViewModelBase
    {
        public Visibility DawgVisible => ErrorLogger.Instance.hasErrors() ? 
            Visibility.Visible : Visibility.Hidden;

        public string NumOfErrors => ErrorLogger.Instance.NumberOfErrors + " Errors";

        public string Errors => ErrorLogger.Instance.ErrorMessages;

        public RelayCommand ResetCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ErrorDawgViewModel class.
        /// </summary>
        public ErrorDawgViewModel()
        {
            ResetCommand = new RelayCommand(ResetLog);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void ResetLog()
        {
            ErrorLogger.Instance.ClearLogging();
            RaisePropertyChanged(() => DawgVisible);
            RaisePropertyChanged(() => NumOfErrors);
            RaisePropertyChanged(() => Errors);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ErrorAdded")
            {
                RaisePropertyChanged(() => DawgVisible);
                RaisePropertyChanged(() => NumOfErrors);
                RaisePropertyChanged(() => Errors);
            }
        }
    }
}