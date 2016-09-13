using FocusTreeManager.ViewModel;
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
    /// Logique d'interaction pour LineAdornerView.xaml
    /// </summary>
    public partial class LineAdornerView : UserControl
    {
        public LineAdornerView()
        {
            InitializeComponent();
            //Loaded event is necessary as Adorner is null until control is shown.
            Loaded += GridWithRulerxaml_Loaded;
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        void GridWithRulerxaml_Loaded(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            var rulerAdorner = new LineAdorner(this, (FocusGridViewModel)this.DataContext);
            adornerLayer.Add(rulerAdorner);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            AdornerLayer.GetAdornerLayer(this).Update();
        }
    }
}
