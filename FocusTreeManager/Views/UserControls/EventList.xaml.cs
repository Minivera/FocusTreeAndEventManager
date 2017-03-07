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
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(resourceLocalization);
        }
    }
}
