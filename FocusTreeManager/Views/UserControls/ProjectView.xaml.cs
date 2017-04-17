using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FocusTreeManager.Helper;
using FocusTreeManager.Model.TabModels;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
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
            //If key is not enter
            if (e.Key != Key.Enter) return;
            Keyboard.ClearFocus();
            ((TextBox)sender).Visibility = Visibility.Hidden;
        }

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            StackPanel parent = null;
            if (item != null)
            {
                parent = ((ContextMenu)item.Parent).PlacementTarget as StackPanel;
            }
            TextBox textbox = UiHelper.FindVisualChildren<TextBox>(parent).FirstOrDefault();
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
            ProjectViewViewModel vm = DataContext as ProjectViewViewModel;
            if (e.NewValue is FocusGridModel ||
                e.NewValue is EventTabModel ||
                e.NewValue is LocalisationModel ||
                e.NewValue is ScriptModel)
            {
                if (vm != null) vm.SelectedItem = e.NewValue;
                RenameButton.IsEnabled = true;
            }
            else
            {
                if (vm != null) vm.SelectedItem = null;
                RenameButton.IsEnabled = false;
            }
            vm?.DeleteElementMenuCommand.RaiseCanExecuteChanged();
            vm?.EditElementMenuCommand.RaiseCanExecuteChanged();
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = CodeTreeView.Tag as TreeViewItem;
            if (item == null) return;
            TextBox textbox = UiHelper.FindVisualChildren<TextBox>(item).FirstOrDefault();
            if (textbox != null)
            {
                textbox.Visibility = Visibility.Visible;
                textbox.Focus();
            }
        }

        private void CodeTreeView_Selected(object sender, RoutedEventArgs e)
        {
            CodeTreeView.Tag = e.OriginalSource;
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox?.Visibility == Visibility.Visible)
            {
                textbox.Visibility = Visibility.Hidden;
            }
        }
    }
}
