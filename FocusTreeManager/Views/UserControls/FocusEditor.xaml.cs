using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    public partial class FocusEditor : UserControl
    {
        public FocusEditor()
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
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void DescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Localizator.Visibility == Visibility.Hidden)
            {
                Localizator.Hide();
                Canvas.SetRight(Localizator, -20);
                Canvas.SetTop(Localizator, Canvas.GetTop(DescriptionButton)
                                           - DescriptionButton.Height);
                DescriptionButton.Command.Execute(DescriptionButton.CommandParameter);
                Localizator.Show();
            }
            else 
            {
                Localizator.Hide();
            }
        }

        private void VisibleNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (Localizator.Visibility == Visibility.Hidden)
            {
                Localizator.Hide();
                Canvas.SetRight(Localizator, -20);
                Canvas.SetTop(Localizator, Canvas.GetTop(VisibleNameButton)
                                - VisibleNameButton.Height);
                VisibleNameButton.Command.Execute(VisibleNameButton.CommandParameter);
                Localizator.Show();
            }
            else 
            {
                Localizator.Hide();
            }
        }
    }
}
