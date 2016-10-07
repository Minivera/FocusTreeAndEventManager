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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Interaction logic for Focus.xaml
    /// </summary>
    public partial class Focus : UserControl
    {
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
            Model.Focus model = this.DataContext as Model.Focus;
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Grid ))
            {
                if (parent == null)
                {
                    dispatcherTimer.Stop();
                    return;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            Point position = this.TranslatePoint(new Point(0, 0), (FrameworkElement)parent);
            model.setPoints(new Point(position.X + (this.RenderSize.Width / 2), position.Y + 40), 
                            new Point(position.X + (this.RenderSize.Width / 2), position.Y + (this.RenderSize.Height)),
                            new Point(position.X, position.Y + (this.RenderSize.Height / 2)),
                            new Point(position.X + (this.RenderSize.Width), position.Y + (this.RenderSize.Height / 2)));
        }
    }
}
