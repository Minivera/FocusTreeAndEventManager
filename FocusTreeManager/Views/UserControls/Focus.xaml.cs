using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Interaction logic for Focus.xaml
    /// </summary>
    public partial class Focus : UserControl
    {
        private Point OldPoint;

        private DispatcherTimer dispatcherTimer;

        public Focus()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Timer
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DetectPositionPoints();
        }

        public void DetectPositionPoints()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Grid))
            {
                if (parent == null)
                {
                    dispatcherTimer.Stop();
                    return;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            Point position = this.TranslatePoint(new Point(1, 1), (FrameworkElement)parent);
            //If the focus has not changed position
            if (OldPoint != null && OldPoint == position)
            {
                return;
            }
            OldPoint = position;
            FocusModel model = this.DataContext as FocusModel;
            model.setPoints(new Point(position.X + (this.RenderSize.Width / 2), position.Y + 40),
                            new Point(position.X + (this.RenderSize.Width / 2), position.Y + (this.RenderSize.Height)),
                            new Point(position.X, position.Y + (this.RenderSize.Height / 2)),
                            new Point(position.X + (this.RenderSize.Width), position.Y + (this.RenderSize.Height / 2)));
            ((FocusGridModel)((FrameworkElement)parent).DataContext).UpdateFocus(model);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBoxName.Visibility = Visibility.Hidden;
                this.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void MenuRenameFocus_Click(object sender, RoutedEventArgs e)
        {
            TextBoxName.Visibility = Visibility.Visible;
            this.CaptureMouse();
        }

        private void VisualFocus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point pos = e.GetPosition(MainWindow.GetWindow(this));
                Rect rect = this.TransformToVisual(MainWindow.GetWindow(this))
                                .TransformBounds(LayoutInformation.GetLayoutSlot(this));
                if (!rect.Contains(pos))
                {
                    TextBoxName.Visibility = Visibility.Hidden;
                    this.ReleaseMouseCapture();
                }
            }
        }
    }
}
