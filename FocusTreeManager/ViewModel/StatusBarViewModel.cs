using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// Will receive message to add to the status bar
    /// </summary>
    public class StatusBarViewModel : ViewModelBase
    {
        private string message;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (message == value)
                {
                    return;
                }
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public StatusBarViewModel()
        {
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }
        
        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            if (msg.Notification == "Clear_message")
            {
                Message = "";
                return;
            }
            string Notification = LocalizationHelper.getValueForKey(msg.Notification);
            if (string.IsNullOrEmpty(Notification)) return;
            Message = Notification;
            if (msg.Target == this)
            {
                //Resend to the tutorial View model if this was the target
                Messenger.Default.Send(new NotificationMessage(msg.Sender,
                new ViewModelLocator().Tutorial, msg.Notification));
            }
        }
    }
}