using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FocusTreeManager.Views
{
    public partial class Localizator : UserControl
    {
        public Localizator()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "CloseLocalizator")
            {
                Storyboard sb = Resources["HideRight"] as Storyboard;
                sb.Begin(this);
                this.Visibility = Visibility.Hidden;
            }
        }

        private void loadLocales()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
            Storyboard sb = Resources["ShowRight"] as Storyboard;
            sb.Begin(this);
            this.Focus();
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            //Storyboard sb = Resources["HideRight"] as Storyboard;
            //sb.Begin(this);
            //this.Visibility = Visibility.Hidden;
        }

        private void TextboxLocale_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                Storyboard sb = Resources["HideRight"] as Storyboard;
                sb.Begin(this);
                this.Visibility = Visibility.Hidden;
                e.Handled = true;
                LocalizatorViewModel vm = this.DataContext as LocalizatorViewModel;
                vm.OkCommand.Execute(null);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                Storyboard sb = Resources["HideRight"] as Storyboard;
                sb.Begin(this);
                this.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
        }
    }
}
