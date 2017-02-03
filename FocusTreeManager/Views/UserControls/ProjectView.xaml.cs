using FocusTreeManager.Helper;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Views
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
            loadLocales();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                ((TextBox)sender).BorderThickness = new Thickness(0);
                ((TextBox)sender).IsReadOnly = true;
            }
        }

        private void loadLocales()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Label Parent = null;
            if (item != null)
            {
                Parent = ((ContextMenu)item.Parent).PlacementTarget as Label;
            }
            TextBox textbox = UiHelper.FindVisualChildren<TextBox>(Parent).FirstOrDefault();
            if (textbox != null)
            {
                textbox.BorderThickness = new Thickness(1);
                textbox.IsReadOnly = false;
            }
        }
    }
}
