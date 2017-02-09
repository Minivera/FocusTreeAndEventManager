using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace FocusTreeManager.Views
{
    public enum ModeType
    {
        Create,
        Edit
    }

    public partial class FileManager : MetroWindow
    {
        public FileManager(ModeType mode)
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            if (mode == ModeType.Edit)
            {
                Column1.Width = new GridLength(0, GridUnitType.Pixel);
                Column2.Width = new GridLength(0, GridUnitType.Pixel);
                FileEditor.Visibility = Visibility.Visible;
            }
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

        private void FileChoiceList_SelectionChanged(object sender, 
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //If new file
            if (FileChoiceList.SelectedIndex == 0)
            {
                FileTypeList.Visibility = Visibility.Visible;
            }
            //If open file
            else if (FileChoiceList.SelectedIndex == 1)
            {
                FileEditor.Visibility = Visibility.Visible;
                FileTypeList.Visibility = Visibility.Hidden;
            }
            else
            {
                FileTypeList.Visibility = Visibility.Hidden;
            }
        }

        private void FileTypeList_SelectionChanged(object sender, 
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FileTypeList.SelectedIndex != -1)
            {
                FileEditor.Visibility = Visibility.Visible;
            }
            else
            {
                FileEditor.Visibility = Visibility.Hidden;
            }
        }
    }
}
