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
using Microsoft.WindowsAPICodePack.Dialogs;
using FocusTreeManager.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

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
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ShowAddFocus")
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
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
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                FocusFlyout.Header = resourceLocalization["Edit_Focus"] as string;
                FocusFlyout.DataContext = (new ViewModelLocator()).EditFocus_Flyout;
                ((EditFocusViewModel)FocusFlyout.DataContext).Focus = (Model.Focus)msg.Sender;
                FocusFlyout.IsOpen = true;
            }
            if (msg.Notification == "HideEditFocus")
            {
                FocusFlyout.IsOpen = false;
            }
            if (msg.Notification == "ShowProjectControl")
            {
                ProjectFlyout.IsOpen = true;
            }
            if (msg.Notification == "HideProjectControl")
            {
                ProjectFlyout.IsOpen = false;
                ProjectFlyout.CloseButtonVisibility = System.Windows.Visibility.Hidden;
            }
            if (msg.Notification == "RefreshTabViewer")
            {
                ((ObservableCollection<ObservableObject>)CentralTabControl.ItemsSource).Clear();
            }
            if (msg.Notification == "ShowChangeImage")
            {
                ChangeImage view = new ChangeImage();
                view.ShowDialog();
            }
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
            if (msg.Notification == "ErrorSavingProject")
            {
                //ShowLoadingErrorDialog();
            }
            if (msg.Notification == "ErrorLoadingProject")
            {
                //ShowSavingErrorDialog();
            }
            if (msg.Notification == "ConfirmBeforeContinue")
            {
                ConfirmBeforeContinue();
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        async private void ConfirmBeforeContinue()
        {
            MessageDialogResult Result = await ShowContinueDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "SaveProject"));
                Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "ContinueCommand"));
            }
            else if (Result == MessageDialogResult.FirstAuxiliary)
            {
                Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "ContinueCommand"));
            }
        }

        async private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            MessageDialogResult Result = await ShowSaveDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Main, "SaveProject"));
                //Show Save dialog and quit
                Application.Current.Shutdown();
            }
            else if (Result == MessageDialogResult.FirstAuxiliary)
            {
                //Quit without saving
                Application.Current.Shutdown();
            }
        }

        async private Task<MessageDialogResult> ShowContinueDialog()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            string Title = resourceLocalization["Exit_Confirm_Title"] as string;
            string Message = resourceLocalization["Delete_Confirm"] as string;
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = resourceLocalization["Command_Save"] as string;
            settings.NegativeButtonText = resourceLocalization["Command_Cancel"] as string;
            settings.FirstAuxiliaryButtonText = resourceLocalization["Command_Continue"] as string;
            return await this.ShowMessageAsync(Title, Message,
                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
        }

        async private Task<MessageDialogResult> ShowSaveDialog()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            string Title = resourceLocalization["Exit_Confirm_Title"] as string;
            string Message = resourceLocalization["Exit_Confirm"] as string;
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = resourceLocalization["Command_Save"] as string;
            settings.NegativeButtonText = resourceLocalization["Command_Cancel"] as string;
            settings.FirstAuxiliaryButtonText = resourceLocalization["Command_Quit"] as string;
            return await this.ShowMessageAsync(Title, Message, 
                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings view = new Settings();
            view.ShowDialog();
        }

        private void loadLocales()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectFlyout.IsOpen = true;
            ProjectFlyout.CloseButtonVisibility = System.Windows.Visibility.Visible;
        }

        //Drag with the mouse effect

        private Point scrollMousePoint = new Point();

        private double hOff = 1;

        private double vOff = 1;

        private bool isMouseHold = false;

        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                return;
            }
            ScrollViewer element = sender as ScrollViewer;
            isMouseHold = true;
            scrollMousePoint = e.GetPosition(element);
            hOff = element.HorizontalOffset;
            vOff = element.VerticalOffset;
        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer element = sender as ScrollViewer;
            isMouseHold = false;
            element.ReleaseMouseCapture();
        }

        private void ContentScroll_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ScrollViewer element = sender as ScrollViewer;
            if (isMouseHold)
            {
                element.CaptureMouse();
                element.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(element).X));
                element.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(element).Y));
            }
        }

        private void CentralTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in ((TabControl)e.Source).Items)
            {
                if (item is FocusGridModel)
                {
                    ((FocusGridModel)item).isShown = false;
                }
            }
            if (e.AddedItems.Count > 0)
            {
                var selectedTab = e.AddedItems[0];
                if (selectedTab is FocusGridModel)
                {
                    ((FocusGridModel)selectedTab).RedrawGrid();
                    ((FocusGridModel)selectedTab).isShown = true;
                }
            }
        }
    }
}
