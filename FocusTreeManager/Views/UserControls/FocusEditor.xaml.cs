using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FocusTreeManager.Views
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
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void DescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetRight(Localizator, -20);
            Canvas.SetTop(Localizator, Canvas.GetTop(DescriptionButton) 
                            - DescriptionButton.Height);
            DescriptionButton.Command.Execute(DescriptionButton.CommandParameter);
            Localizator.Show();
        }

        private void VisibleNameButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetRight(Localizator, -20);
            Canvas.SetTop(Localizator, Canvas.GetTop(VisibleNameButton)
                            - VisibleNameButton.Height);
            VisibleNameButton.Command.Execute(VisibleNameButton.CommandParameter);
            Localizator.Show();
        }
    }
}
