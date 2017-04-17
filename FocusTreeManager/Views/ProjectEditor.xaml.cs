using FocusTreeManager.Helper;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = LocalizationHelper.getValueForKey("Project_Select"),
                InitialDirectory = "C:",
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = "C:",
                EnsureFileExists = false,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Project", "*.xh4prj"));
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string filename = dialog.FileName;
                if (string.IsNullOrEmpty(Path.GetExtension(filename)))
                {
                    filename += ".xh4prj";
                }
                ((TextBox)sender).Text = filename;
                BindingExpression bindingExpression = ((TextBox)sender)
                    .GetBindingExpression(TextBox.TextProperty);
                bindingExpression?.UpdateSource();
            }
            Activate();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Check Tutorial
            new ViewModelLocator().Tutorial.StartCommand.RaiseCanExecuteChanged();
        }
    }
}
