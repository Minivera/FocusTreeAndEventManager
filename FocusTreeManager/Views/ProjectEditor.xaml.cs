using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FocusTreeManager.Views
{
    public partial class ProjectEditor : MetroWindow
    {
        public ProjectEditor()
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
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            dialog.Title = resourceLocalization["Project_Load"] as string;
            dialog.InitialDirectory = "C:";
            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = "C:";
            dialog.EnsureFileExists = false;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Filters.Add(new CommonFileDialogFilter("Project", "*.xh4prj"));
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ((TextBox)sender).Text = dialog.FileName;
            }
            this.Activate();
        }
    }
}
