using FocusTreeManager.CodeStructures;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using FocusTreeManager.ViewModel;
using static FocusTreeManager.ViewModel.ScripterControlsViewModel;

namespace FocusTreeManager.Views
{
    public partial class EditScript : MetroWindow
    {

        public EditScript()
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
    }
}
