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
using FocusTreeManager.DataContract;

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
                        ResourceDictionary resourceLocalization = new ResourceDictionary();
                        resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                        FocusFlyout.Header = resourceLocalization["Add_Focus"] as string;
                        (new ViewModelLocator()).EditFocus.SetupFlyout(msg.Sender, ModeType.Create);
                        FocusFlyout.IsOpen = true;
                        break;
                    }
                case "ShowEditFocus":
                    {
                        ResourceDictionary resourceLocalization = new ResourceDictionary();
                        resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                        FocusFlyout.Header = resourceLocalization["Edit_Focus"] as string;
                        (new ViewModelLocator()).EditFocus.SetupFlyout(msg.Sender, ModeType.Edit);
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
                        ProjectFlyout.CloseButtonVisibility = System.Windows.Visibility.Hidden;
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

        async private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel Model = this.DataContext as MainViewModel;
            if (Model.IsProjectExist)
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

        async private void SettingsButton_Click(object sender, RoutedEventArgs e)
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

        async private Task<MessageDialogResult> ShowWrongGameFolderDialog()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            string Title = resourceLocalization["Application_Game_Folder_Not_Set_Header"] as string;
            string Message = resourceLocalization["Application_Game_Folder_Not_Set"] as string;
            return await this.ShowMessageAsync(Title, Message, MessageDialogStyle.Affirmative);
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
            //If we hit a focus or a scrollbar rather than an empty grid
            if (((e.OriginalSource is FrameworkElement) && 
                (((FrameworkElement)e.OriginalSource).DataContext is FocusModel ||
                e.OriginalSource is Rectangle)))
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
            if (isMouseHold)
            {
                ScrollViewer element = sender as ScrollViewer;
                isMouseHold = false;
                element.ReleaseMouseCapture();
            }
        }

        private void ContentScroll_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseHold)
            {
                ScrollViewer element = sender as ScrollViewer;
                element.CaptureMouse();
                element.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(element).X));
                element.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(element).Y));
            }
        }

        private void CentralTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabablzControl))
            {
                return;
            }
            foreach (var item in ((TabControl)e.Source).Items)
            {
                if (item is FocusGridModel)
                {
                    ((FocusGridModel)item).isShown = false;
                }
                else if (item is EventModel)
                {
                    ((EventTabModel)item).isShown = false;
                }
                //Save all potential datagrids unsaved rows
                foreach (DataGrid grid
                    in UiHelper.FindVisualChildren<DataGrid>((TabControl)e.Source))
                {
                    grid.CancelEdit();
                }
            }
            if (e.AddedItems.Count > 0)
            {
                var selectedTab = e.AddedItems[0];
                if (selectedTab is FocusGridModel)
                {
                    ((FocusGridModel)selectedTab).isShown = true;
                    foreach (FocusGrid grid in UiHelper.FindVisualChildren<FocusGrid>(CentralTabControl))
                    {
                        if (grid.DataContext == selectedTab)
                        {
                            grid.setupInternalFoci();
                            ((FocusGridModel)selectedTab).RedrawGrid();
                            break;
                        }
                    }
                }
                else if (selectedTab is EventModel)
                {
                    ((EventTabModel)selectedTab).isShown = true;
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //If there is a command line argument
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var fileName = args[1];
                if (File.Exists(fileName))
                {
                    var extension = System.IO.Path.GetExtension(fileName);
                    //If a file was opened and the fil is a project
                    if (extension == ".h4prj" || extension == ".xh4prj")
                    {
                        //Load it
                        MainViewModel vm = this.DataContext as MainViewModel;
                        vm.LoadProject(fileName);
                    }
                }
            }
            if (!Directory.Exists(Configurator.getGamePath() + @"\gfx\interface\"))
            {
                SettingsButton_Click(this, new RoutedEventArgs());
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            int RowsToAdd = 0;
            int ColumnsToAdd = 0;
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                //Max Vertical
                RowsToAdd++;
            }
            if (scrollViewer.HorizontalOffset == scrollViewer.ScrollableWidth)
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
