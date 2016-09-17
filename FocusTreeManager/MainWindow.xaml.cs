using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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

namespace FocusTreeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Localization locale = new Localization();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ShowAddFocus")
            {
                Localization locale = new Localization();
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
                FocusFlyout.Header = resourceLocalization["Add_Focus"] as string;
                FocusFlyout.DataContext = (new ViewModelLocator()).AddFocus_Flyout.SetupFlyout();
                FocusFlyout.IsOpen = true;
            }
            if (msg.Notification == "HideAddFocus")
            {
                FocusFlyout.IsOpen = false;
            }
            if (msg.Notification == "ShowEditFocus")
            {
                Localization locale = new Localization();
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
                FocusFlyout.Header = resourceLocalization["Edit_Focus"] as string;
                FocusFlyout.DataContext = (new ViewModelLocator()).EditFocus_Flyout;
                ((EditFocusViewModel)FocusFlyout.DataContext).Focus = (Model.Focus)msg.Sender;
                FocusFlyout.IsOpen = true;
            }
            if (msg.Notification == "HideEditFocus")
            {
                FocusFlyout.IsOpen = false;
            }
            if (msg.Notification == "ShowChangeImage")
            {
                ChangeImage view = new ChangeImage();
                view.ShowDialog();
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        async private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            MessageDialogResult Result = await ShowSaveDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                //Show Save dialog and quit
                Application.Current.Shutdown();
            }
            else if (Result == MessageDialogResult.FirstAuxiliary)
            {
                //Quit without saving
                Application.Current.Shutdown();
            }
        }

        async private Task<MessageDialogResult> ShowSaveDialog()
        {
            Localization locale = new Localization();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
            string Title = resourceLocalization["Exit_Confirm_Title"] as string;
            string Message = resourceLocalization["Exit_Confirm"] as string;
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = resourceLocalization["Command_Save"] as string;
            settings.NegativeButtonText = resourceLocalization["Command_Cancel"] as string;
            settings.FirstAuxiliaryButtonText = resourceLocalization["Command_Quit"] as string;
            return await this.ShowMessageAsync(Title, Message, 
                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
        }
    }
}
