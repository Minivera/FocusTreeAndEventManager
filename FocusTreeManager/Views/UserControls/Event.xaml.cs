using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Event : UserControl
    {
        public Event()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
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

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                EventModel model = DataContext as EventModel;
                if (model == null) return;
                model.EditDescScriptCommand.RaiseCanExecuteChanged();
                model.EditOptionScriptCommand.RaiseCanExecuteChanged();
            }), DispatcherPriority.ContextIdle, null);
        }
    }
}
