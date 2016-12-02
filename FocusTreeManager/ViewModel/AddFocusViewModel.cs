using FocusTreeManager.CodeStructures;
using FocusTreeManager.Model;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
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
        /// Initializes a new instance of the AddFocusViewModel class.
        /// </summary>
        public AddFocusViewModel()
        {
            FocusCommand = new RelayCommand(AddFocus);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            EditScriptCommand = new RelayCommand(EditScript);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public AddFocusViewModel SetupFlyout(object sender)
        {
            try
            {
                Point mousePos = Mouse.GetPosition((IInputElement)sender);
                Model.Focus focus = new Model.Focus();
                focus.setDefaults(((FocusGridModel)(new ViewModelLocator()).Main.TabsModelList
                                    .FirstOrDefault((t) => t is FocusGridModel && ((FocusGridModel)t).isShown))
                                    .FociList.Count());
                Focus = focus;
                //minus 0.4 because if you hit the border of a cell, it will add it to the next one... Annoying
                Focus.X = (int)Math.Floor((mousePos.X / 89) - 0.4);
                Focus.Y = (int)Math.Floor(mousePos.Y / 140);
                RaisePropertyChanged("Focus");
            }
            catch (Exception)
            {
            }
            return this;
        }

        public void AddFocus()
        {
            Messenger.Default.Send(new NotificationMessage(this, "HideAddFocus"));
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