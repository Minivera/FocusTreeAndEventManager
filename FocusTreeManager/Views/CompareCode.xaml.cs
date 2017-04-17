using FocusTreeManager.Helper;
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
            MaxHeight = SystemParameters.PrimaryScreenHeight * 0.90;
            MaxWidth = SystemParameters.PrimaryScreenWidth * 0.90;
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