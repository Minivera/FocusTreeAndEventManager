using System;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Scripter : UserControl
    {
        public Scripter()
        {
            InitializeComponent();
            loadLocales();
            DataContext = new ViewModelLocator().Scripter;
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

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Configurator.getScripterPreference())
            {
                case "Scripter":
                    EditorTab.Visibility = Visibility.Collapsed;
                    ScripterTab.Visibility = Visibility.Visible;
                    break;
                case "Editor":
                    EditorTab.Visibility = Visibility.Visible;
                    ScripterTab.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
