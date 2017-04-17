using FocusTreeManager.Helper;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FocusTreeManager.Views.UserControls
{
    public partial class EventList : UserControl
    {
        public EventList()
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
