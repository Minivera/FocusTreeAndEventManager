using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Description for CompareCode.
    /// </summary>
    public partial class CompareCode : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the CompareCode class.
        /// </summary>
        public CompareCode()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            this.MaxHeight = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.90);
            this.MaxWidth = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.90);
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
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }
    }
}