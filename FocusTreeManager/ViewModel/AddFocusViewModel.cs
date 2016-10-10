using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

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
        /// Initializes a new instance of the AddFocusViewModel class.
        /// </summary>
        public AddFocusViewModel()
        {
            FocusCommand = new RelayCommand(AddFocus);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public AddFocusViewModel SetupFlyout(object sender)
        {
            Point mousePos = Mouse.GetPosition((IInputElement)sender);
            Focus = new Focus();
            Focus.setDefaults();
            //minus 0.4 because if you hit the border of a cell, it will add it to the next one... Anoying
            Focus.X = (int)Math.Floor((mousePos.X / 89) - 0.4);
            Focus.Y = (int)Math.Floor(mousePos.Y / 140);
            RaisePropertyChanged("Focus");
            return this;
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
            if (msg.Target != null && msg.Target != this)
            {
                //Message not itended for here
                return;
            }
            if (msg.Notification == "HideChangeImage")
            {
                if ((string)System.Windows.Application.Current.Properties["Mode"] == "Add")
                { 
                    ChangeImageViewModel viewModel = (ChangeImageViewModel)msg.Sender;
                    Focus.Image = viewModel.FocusImage;
                }
            }
        }
    }
}