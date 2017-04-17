using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Views.UserControls
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
                sb?.Begin(this);
                Visibility = Visibility.Hidden;
            }
        }

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            Storyboard sb = Resources["ShowRight"] as Storyboard;
            sb?.Begin(this);
            Focus();
        }

        public void Hide()
        {
            Storyboard sb = Resources["HideRight"] as Storyboard;
            sb?.Begin(this);
            Visibility = Visibility.Hidden;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            //Storyboard sb = Resources["HideRight"] as Storyboard;
            //sb.Begin(this);
            //this.Visibility = Visibility.Hidden;
        }

        private void TextboxLocale_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Return:
                {
                    Hide();
                    e.Handled = true;
                    LocalizatorViewModel vm = DataContext as LocalizatorViewModel;
                    vm?.OkCommand.Execute(null);
                    break;
                }
                case System.Windows.Input.Key.Escape:
                {
                    Hide();
                    e.Handled = true;
                    break;
                }
            }
        }
    }
}
