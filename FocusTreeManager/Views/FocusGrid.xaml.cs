using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
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

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Interaction logic for FocusGrid.xaml
    /// </summary>
    public partial class FocusGrid : UserControl
    {
        public FocusGrid()
        {
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
            LineAdorner Adorner = new LineAdorner(ListGrid, (FocusGridModel)this.DataContext);
            adornerLayer.Add(Adorner);
        }

        private void loadLocales()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
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
            Focus element = sender as Focus;
            if (element != null && isDown && e.LeftButton == MouseButtonState.Pressed)
            {
                DraggedElement = element;
                DraggedElement.CaptureMouse();
            }
            if (DraggedElement != null && DraggedElement.IsMouseCaptured)
            {
                currentPoint = e.GetPosition(ListGrid);
                transform.X += currentPoint.X - anchorPoint.X;
                transform.Y += (currentPoint.Y - anchorPoint.Y);
                DraggedElement.RenderTransform = transform;
                anchorPoint = currentPoint;
            }
        }

        private void Focus_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedElement == null)
            {
                return;
            }
            DraggedElement.ReleaseMouseCapture();
            ((FocusGridModel)this.DataContext).ChangePosition(DraggedElement.DataContext, currentPoint);
            transform.X = 0;
            transform.Y = 0;
            DraggedElement.RenderTransform = new TranslateTransform();
            isDown = false;
            DraggedElement = null;
            e.Handled = true;
        }
    }
}
