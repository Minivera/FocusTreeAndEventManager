using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
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
                ((TextBox)sender).Visibility = Visibility.Hidden;
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
            StackPanel Parent = null;
            if (item != null)
            {
                Parent = ((ContextMenu)item.Parent).PlacementTarget as StackPanel;
            }
            TextBox textbox = UiHelper.FindVisualChildren<TextBox>(Parent).FirstOrDefault();
            if (textbox != null)
            {
                textbox.Visibility = Visibility.Visible;
            }
        }

        private void CodeTreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void CodeTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ProjectViewViewModel vm = this.DataContext as ProjectViewViewModel;
            if (e.NewValue is FocusGridModel ||
                e.NewValue is EventTabModel ||
                e.NewValue is LocalisationModel ||
                e.NewValue is ScriptModel)
            {
                vm.SelectedItem = e.NewValue;
                RenameButton.IsEnabled = true;
            }
            else
            {
                vm.SelectedItem = null;
                RenameButton.IsEnabled = false;
            }
            vm.DeleteElementMenuCommand.RaiseCanExecuteChanged();
            vm.EditElementMenuCommand.RaiseCanExecuteChanged();
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = CodeTreeView.Tag as TreeViewItem;
            if (item != null)
            {
                TextBox textbox = UiHelper.FindVisualChildren<TextBox>(item).FirstOrDefault();
                if (textbox != null)
                {
                    textbox.Visibility = Visibility.Visible;
                }
            }
        }

        private void CodeTreeView_Selected(object sender, RoutedEventArgs e)
        {
            CodeTreeView.Tag = e.OriginalSource;
        }
    }
}
