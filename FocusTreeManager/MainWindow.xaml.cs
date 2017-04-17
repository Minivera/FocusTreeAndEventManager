using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using FocusTreeManager.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.IO;
using FocusTreeManager.Helper;
using Dragablz;
using FocusTreeManager.Model.TabModels;
using FocusGrid = FocusTreeManager.Views.UserControls.FocusGrid;

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
            //If the app has never been started
            if (!Configurator.getFirstStart())
            {
                Settings view = new Settings();
                view.ShowDialog();
                Configurator.setFirstStart();
            }
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            switch (msg.Notification)
            {
                case "ShowAddFocus":
                    {
                        FocusFlyout.Header = LocalizationHelper.getValueForKey("Add_Focus");
                        new ViewModelLocator().EditFocus.SetupFlyout(msg.Sender, ModeType.Create);
                        FocusFlyout.IsOpen = true;
                        break;
                    }
                case "ShowEditFocus":
                    {
                        FocusFlyout.Header = LocalizationHelper.getValueForKey("Edit_Focus");
                        new ViewModelLocator().EditFocus.SetupFlyout(msg.Sender, ModeType.Edit);
                        FocusFlyout.IsOpen = true;
                        break;
                    }
                case "CloseEditFocus":
                    {
                        FocusFlyout.IsOpen = false;
                        break;
                    }
                case "ShowProjectControl":
                    {
                        ProjectFlyout.IsOpen = true;
                        break;
                    }
                case "HideProjectControl":
                    {
                        ProjectFlyout.IsOpen = false;
                        ProjectFlyout.CloseButtonVisibility = Visibility.Hidden;
                        break;
                    }
                case "RefreshTabViewer":
                    {
                        ((ObservableCollection<ObservableObject>)CentralTabControl.ItemsSource).Clear();
                        break;
                    }
                case "ChangeLanguage":
                    {
                        loadLocales();
                        break;
                    }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel Model = DataContext as MainViewModel;
            //If the model or project do not exist, quit.
            if (Model == null || !Model.IsProjectExist) return;
            e.Cancel = true;
            MessageDialogResult Result = await ShowSaveDialog();
            switch (Result)
            {
                case MessageDialogResult.Affirmative:
                    Messenger.Default.Send(new NotificationMessage(this, 
                        new ViewModelLocator().Main, "SaveProject"));
                    //Show Save dialog and quit
                    Application.Current.Shutdown();
                    break;
                case MessageDialogResult.FirstAuxiliary:
                    //Quit without saving
                    Application.Current.Shutdown();
                    break;
            }
        }

        private async Task<MessageDialogResult> ShowSaveDialog()
        {
            string title = LocalizationHelper.getValueForKey("Exit_Confirm");
            string Message = LocalizationHelper.getValueForKey("Exit_Confirm_Title");
            MetroDialogSettings settings = new MetroDialogSettings
            {
                AffirmativeButtonText = LocalizationHelper.getValueForKey("Command_Save"),
                NegativeButtonText = LocalizationHelper.getValueForKey("Command_Cancel"),
                FirstAuxiliaryButtonText = LocalizationHelper.getValueForKey("Command_Quit")
            };
            return await this.ShowMessageAsync(title, Message, 
                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings view = new Settings();
            if (!Directory.Exists(Configurator.getGamePath() + @"\gfx\interface\"))
            {
                MessageDialogResult result = await ShowWrongGameFolderDialog();
                if (result == MessageDialogResult.Affirmative)
                {
                    view.ShowDialog();
                }
            }
            else
            {
                view.ShowDialog();
            }
        }

        private async Task<MessageDialogResult> ShowWrongGameFolderDialog()
        {
            string title = LocalizationHelper.getValueForKey("Application_Game_Folder_Not_Set_Header");
            string Message = LocalizationHelper.getValueForKey("Application_Game_Folder_Not_Set");
            return await this.ShowMessageAsync(title, Message);
        }

        private void loadLocales()
        {
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectFlyout.IsOpen = true;
            ProjectFlyout.CloseButtonVisibility = Visibility.Visible;
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().Tutorial, "OpenedProjectFylout"));
        }

        private void ProjectFlyout_OnClosingFinished(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().Tutorial, "ClosedProjectFylout"));
        }

        //Drag with the mouse effect

        private Point scrollMousePoint;

        private double hOff = 1;

        private double vOff = 1;

        private bool isMouseHold;

        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Make sure we hold the middle mouse button
            if (e.ChangedButton != MouseButton.Middle) return;
            //If we hit a focus or a scrollbar rather than an empty grid
            FrameworkElement source = e.OriginalSource as FrameworkElement;
            if (source != null && 
                (source.DataContext is FocusModel ||
                 e.OriginalSource is Rectangle))
            {
                return;
            }
            ScrollViewer element = sender as ScrollViewer;
            isMouseHold = true;
            Cursor = ((TextBlock)Resources["CursorGrabbing"]).Cursor;
            scrollMousePoint = e.GetPosition(element);
            //If element could not be found
            if (element == null) return;
            hOff = element.HorizontalOffset;
            vOff = element.VerticalOffset;
        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Make sure we hold the middle mouse button
            if (e.ChangedButton != MouseButton.Middle) return;
            //If the mouse is not held
            if (!isMouseHold) return;
            ScrollViewer element = sender as ScrollViewer;
            isMouseHold = false;
            Cursor = null;
            element?.ReleaseMouseCapture();
        }

        private void ContentScroll_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //Make sure we hold the middle mouse button
            if (e.MiddleButton != MouseButtonState.Pressed) return;
            if (!isMouseHold) return;
            ScrollViewer element = sender as ScrollViewer;
            //If the element could not be found
            if (element == null) return;
            element.CaptureMouse();
            element.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - 
                                                     e.GetPosition(element).X));
            element.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - 
                                                   e.GetPosition(element).Y));
        }

        private void CentralTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabablzControl))
            {
                return;
            }
            foreach (object item in ((TabControl)e.Source).Items)
            {
                //Save all potential datagrids unsaved rows
                foreach (DataGrid grid
                    in UiHelper.FindVisualChildren<DataGrid>((TabControl)e.Source))
                {
                    grid.CancelEdit();
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //If there is a command line argument
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string fileName = args[1];
                if (File.Exists(fileName))
                {
                    string extension = System.IO.Path.GetExtension(fileName);
                    //If a file was opened and the fil is a project
                    if (extension == ".h4prj" || extension == ".xh4prj")
                    {
                        //Load it
                        MainViewModel vm = DataContext as MainViewModel;
                        vm?.LoadProject(fileName);
                    }
                }
            }
            if (!Directory.Exists(Configurator.getGamePath() + @"\gfx\interface\"))
            {
                SettingsButton_Click(this, new RoutedEventArgs());
            }
            //Check Tutorial
            new ViewModelLocator().Tutorial.StartCommand.RaiseCanExecuteChanged();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            int RowsToAdd = 0;
            int ColumnsToAdd = 0;
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer?.VerticalOffset == scrollViewer?.ScrollableHeight)
            {
                //Max Vertical
                RowsToAdd++;
            }
            if (scrollViewer?.HorizontalOffset == scrollViewer?.ScrollableWidth)
            {
                //Max horizontal
                ColumnsToAdd++;
            }
            foreach (FocusGrid grid in UiHelper.FindVisualChildren<FocusGrid>((FrameworkElement)sender))
            {
                ((FocusGridModel)grid.DataContext).AddGridCells(RowsToAdd, ColumnsToAdd);
            }
        }
    }
}
