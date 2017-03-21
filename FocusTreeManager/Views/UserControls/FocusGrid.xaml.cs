using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FocusTreeManager.Adorners;
using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using FocusTreeManager.Model.TabModels;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    /// <summary>
    /// Interaction logic for FocusGrid.xaml
    /// </summary>
    public partial class FocusGrid : UserControl
    {
        public FocusGrid()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            InitializeComponent();
            loadLocales();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "DrawOnCanvas")
            {
                AdornerLayer.GetAdornerLayer(ListGrid).Update();
            }
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Adorner
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(ListGrid);
            LineAdorner Adorner = new LineAdorner(ListGrid, (FocusGridModel)DataContext);
            adornerLayer.Add(Adorner);
            setupInternalFoci();
            Mouse.OverrideCursor = null;
            //Keydown
            Window MainWindow = Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(x => x.IsActive);
            MainWindow.KeyDown += FocusGrid_OnKeyDown;
            MainWindow.KeyUp += FocusGrid_OnKeyUp;
        }

        public void setupInternalFoci()
        {
            //Build display
            foreach (Focus focus in UiHelper.FindVisualChildren<Focus>(ListGrid))
            {
                focus.DetectPositionPoints();
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

        private Point anchorPoint;
        private Point currentPoint;
        private Focus DraggedElement;
        private bool isDown;

        private void Focus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            anchorPoint = e.GetPosition(ListGrid);
            isDown = true;
        }

        private readonly TranslateTransform transform = new TranslateTransform();

        private void Focus_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is TextBox)
            {
                return;
            }
            Focus element = sender as Focus;
            if (element != null && isDown && e.LeftButton == MouseButtonState.Pressed)
            {
                DraggedElement = element;
                DraggedElement.CaptureMouse();
            }
            if (DraggedElement == null || !DraggedElement.IsMouseCaptured) return;
            currentPoint = e.GetPosition(ListGrid);
            transform.X += currentPoint.X - anchorPoint.X;
            transform.Y += (currentPoint.Y - anchorPoint.Y);
            DraggedElement.RenderTransform = transform;
            anchorPoint = currentPoint;
        }

        private void Focus_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedElement == null)
            {
                return;
            }
            DraggedElement.ReleaseMouseCapture();
            ((FocusGridModel)DataContext).ChangePosition(DraggedElement.DataContext, currentPoint);
            transform.X = 0;
            transform.Y = 0;
            DraggedElement.RenderTransform = new TranslateTransform();
            isDown = false;
            DraggedElement = null;
            e.Handled = true;
        }

        private bool IsShown;

        private void FocusGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (IsShown && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                IsShown = false;
                FocusGridModel model = (FocusGridModel) DataContext;
                model.ShowHidePositionLinesCommand.Execute(this);
            }
        }

        private void FocusGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsShown && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                IsShown = true;
                FocusGridModel model = (FocusGridModel)DataContext;
                model.ShowHidePositionLinesCommand.Execute(this);
            }
        }
    }
}
