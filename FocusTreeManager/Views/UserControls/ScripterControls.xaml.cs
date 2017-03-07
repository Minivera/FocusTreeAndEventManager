using System;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    public partial class ScripterControls : UserControl
    {
        public static readonly DependencyProperty ScripterTypeProperty =
        DependencyProperty.Register("CurrentType", typeof(ScripterControlsViewModel.ScripterType), typeof(ScripterControls),
        new UIPropertyMetadata(ScripterControlsViewModel.ScripterType.Generic));

        public ScripterControlsViewModel.ScripterType CurrentType
        {
            get { return (ScripterControlsViewModel.ScripterType)GetValue(ScripterTypeProperty); }
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
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(resourceLocalization);
        }
    }
}
