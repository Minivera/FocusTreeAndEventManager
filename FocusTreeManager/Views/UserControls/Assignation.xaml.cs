using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Assignation : UserControl
    {
        public Assignation()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
        }

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).BorderThickness = new Thickness(1);
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).BorderThickness = new Thickness(0);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //If the key is not enter
            if (e.Key != Key.Enter) return;
            Keyboard.ClearFocus();
            ((TextBox)sender).BorderThickness = new Thickness(0);
        }
    }
}
