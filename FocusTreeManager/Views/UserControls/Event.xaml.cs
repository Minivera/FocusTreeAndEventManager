using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;


namespace FocusTreeManager.Views.UserControls
{
    public partial class Event : UserControl
    {
        public Event()
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

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            object dataContext = ((FrameworkElement)sender).DataContext;
            if (dataContext is EventOptionModel || dataContext is EventDescriptionModel)
            {
                ((FrameworkElement)sender).IsEnabled = true;
            }
        }

        private void TitleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Localizator.Visibility == Visibility.Hidden)
            {
                Localizator.Hide();
                Canvas.SetRight(Localizator, Canvas.GetRight(TitleButton) - 20);
                Canvas.SetTop(Localizator, Canvas.GetTop(TitleButton)
                                           + TitleButton.Height + 11);
                TitleButton.Command.Execute(TitleButton.CommandParameter);
                Localizator.setPosition(Position.Left);
                Localizator.setPosition(Position.Top);
                Localizator.ShowWithoutAnim();
            }
            else
            {
                Localizator.Hide();
            }
        }

        private void DescriptionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Localizator.Visibility == Visibility.Hidden)
            {
                Localizator.Hide();
                Canvas.SetRight(Localizator, Canvas.GetRight(DescriptionButton) - 20);
                Canvas.SetTop(Localizator, Canvas.GetTop(DescriptionButton)
                                           - DescriptionButton.Height);
                DescriptionButton.Command.Execute(DescriptionButton.CommandParameter);
                Localizator.setPosition(Position.Left);
                Localizator.setPosition(Position.Bottom);
                Localizator.ShowWithoutAnim();
            }
            else
            {
                Localizator.Hide();
            }
        }
    }
}
