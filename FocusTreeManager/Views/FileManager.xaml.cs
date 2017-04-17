using FocusTreeManager.Helper;
using FocusTreeManager.ViewModel;
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
                TutorialButton.Visibility = Visibility.Hidden;
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
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void FileChoiceList_SelectionChanged(object sender, 
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //If new file
            switch (FileChoiceList.SelectedIndex)
            {
                case 0:
                    FileEditor.Visibility = Visibility.Hidden;
                    FileTypeList.Visibility = Visibility.Visible;
                    Messenger.Default.Send(new NotificationMessage(this,
                        new ViewModelLocator().Tutorial, "ChosenNewFile"));
                    break;
                case 1:
                    FileEditor.Visibility = Visibility.Visible;
                    FileTypeList.Visibility = Visibility.Hidden;
                    Messenger.Default.Send(new NotificationMessage(this,
                        new ViewModelLocator().Tutorial, "ChosenImportFile"));
                    break;
                default:
                    FileTypeList.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void FileTypeList_SelectionChanged(object sender, 
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FileEditor.Visibility = FileTypeList.SelectedIndex != -1 ? 
                Visibility.Visible : Visibility.Hidden;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Check Tutorial
            new ViewModelLocator().Tutorial.StartCommand.RaiseCanExecuteChanged();
        }
    }
}
