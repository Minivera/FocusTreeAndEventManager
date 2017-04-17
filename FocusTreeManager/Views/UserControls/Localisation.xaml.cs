using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;
using FocusTreeManager.ViewModel;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Localisation : UserControl
    {
        private enum ColumnToFilter
        {
            Key,
            Value
        }

        private TextBox FilterKeyTextBox;

        private TextBox FilterValueTextBox;

        public Localisation()
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

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private ColumnToFilter FilterKey;

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            LocaleModel item = e.Item as LocaleModel;
            if (item == null) return;
            if (FilterKey == ColumnToFilter.Key && 
                !string.IsNullOrEmpty(FilterKeyTextBox?.Text))
            {
                e.Accepted = item.Key.ToLower().Contains(FilterKeyTextBox.Text.ToLower());
            }
            else if (FilterKey == ColumnToFilter.Value && 
                !string.IsNullOrEmpty(FilterValueTextBox?.Text))
            {
                e.Accepted = item.Value.ToLower().Contains(FilterValueTextBox.Text.ToLower());
            }
        }

        private void FilterKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterKeyTextBox != null && FilterKeyTextBox.Visibility == Visibility.Collapsed)
            {
                FilterKeyTextBox.Visibility = Visibility.Visible;
            }
            else if(FilterKeyTextBox != null)
            {
                FilterKeyTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void FilterKeyTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            FilterKeyTextBox = sender as TextBox;
        }

        private void FilterKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterKey = ColumnToFilter.Key;
            ((CollectionViewSource)Resources["LocalisationSource"]).View.Refresh();
        }

        private void FilterValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterValueTextBox != null && FilterValueTextBox.Visibility == Visibility.Collapsed)
            {
                FilterValueTextBox.Visibility = Visibility.Visible;
            }
            else if (FilterValueTextBox != null)
            {
                FilterValueTextBox.Visibility = Visibility.Collapsed;
            }
        }
        private void FilterValueTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            FilterValueTextBox = sender as TextBox;
        }
        
        private void FilterValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterKey = ColumnToFilter.Value;
            ((CollectionViewSource)Resources["LocalisationSource"]).View.Refresh();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Check Tutorial
            new ViewModelLocator().Tutorial.StartCommand.RaiseCanExecuteChanged();
        }
    }
}
