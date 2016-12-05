using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FocusTreeManager.Views
{
    public partial class Localisation : UserControl
    {
        enum ColumnToFilter
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
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private ColumnToFilter FilterKey;

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            LocaleModel item = e.Item as LocaleModel;
            if (item != null)
            {
                if (FilterKey == ColumnToFilter.Key && FilterKeyTextBox != null)
                {
                    if (item.Key.ToLower().Contains(FilterKeyTextBox.Text.ToLower()))
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else if (FilterKey == ColumnToFilter.Value && FilterValueTextBox != null)
                {
                    if (item.Value.ToLower().Contains(FilterValueTextBox.Text.ToLower()))
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
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
            ((CollectionViewSource)this.Resources["LocalisationSource"]).View.Refresh();
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
            ((CollectionViewSource)this.Resources["LocalisationSource"]).View.Refresh();
        }
    }
}
