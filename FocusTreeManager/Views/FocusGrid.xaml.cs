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
    }
}
