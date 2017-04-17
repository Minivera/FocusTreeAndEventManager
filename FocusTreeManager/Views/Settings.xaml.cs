using FocusTreeManager.Helper;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace FocusTreeManager.Views
{
    public partial class Settings : MetroWindow
    {
        public Settings()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }
    }
}
