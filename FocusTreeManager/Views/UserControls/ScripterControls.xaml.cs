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
using static FocusTreeManager.ViewModel.ScripterControlsViewModel;

namespace FocusTreeManager.Views
{
    public partial class ScripterControls : UserControl
    {
        public static readonly DependencyProperty ScripterTypeProperty =
        DependencyProperty.Register("CurrentType", typeof(ScripterType), typeof(ScripterControls),
        new UIPropertyMetadata(ScripterType.Generic));

        public ScripterType CurrentType
        {
            get { return (ScripterType)GetValue(ScripterTypeProperty); }
            set { SetValue(ScripterTypeProperty, value); }
        }

        public ScripterControls()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Loaded += (s, e) =>
            {
                ScripterControlsViewModel Vm = DataContext as ScripterControlsViewModel;
                if (Vm != null)
                {
                    Vm.CurrentType = CurrentType;
                }
            };
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
    }
}
