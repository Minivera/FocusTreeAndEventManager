using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Event : UserControl
    {
        public Event()
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

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            object dataContext = ((FrameworkElement)sender).DataContext;
            if (dataContext is EventOptionModel || dataContext is EventDescriptionModel)
            {
                ((FrameworkElement)sender).IsEnabled = true;
            }
        }
    }
}
