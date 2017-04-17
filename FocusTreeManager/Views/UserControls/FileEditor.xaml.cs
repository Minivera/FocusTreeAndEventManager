using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Views.UserControls
{
    public partial class FileEditor : UserControl
    {
        public FileEditor()
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
    }
}
