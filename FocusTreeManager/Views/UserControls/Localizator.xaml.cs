using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Views.UserControls
{
    public enum Position
    {
        Left,
        Right,
        Top, 
        Bottom
    }  

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

        public void ShowWithoutAnim()
        {
            Visibility = Visibility.Visible;
            Focus();
        }

        public void Hide()
        {
            Storyboard sb = Resources["HideRight"] as Storyboard;
            sb?.Begin(this);
            Visibility = Visibility.Hidden;
        }

        public void setPosition(Position postion)
        {
            switch (postion)
            {
                    case Position.Bottom:
                        ArrowDownPolygon.Visibility = Visibility.Visible;
                        ArrowUpPolygon.Visibility = Visibility.Hidden;
                        break;
                    case Position.Left:
                        Canvas.SetLeft(ArrowDownPolygon, 20);
                        Canvas.SetLeft(ArrowUpPolygon, 20);
                    break;
                    case Position.Right:
                        Canvas.SetLeft(ArrowDownPolygon, 380);
                        Canvas.SetLeft(ArrowUpPolygon, 380);
                        break;
                    case Position.Top:
                        ArrowDownPolygon.Visibility = Visibility.Hidden;
                        ArrowUpPolygon.Visibility = Visibility.Visible;
                        break;
            }
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
                    LocalizatorViewModel vm = DataContext as LocalizatorViewModel;
                    vm?.OkCommand.Execute(null);
                    Hide();
                    e.Handled = true;
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
