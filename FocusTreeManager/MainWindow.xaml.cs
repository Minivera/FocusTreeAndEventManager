using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
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
            Localization locale = new Localization();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ShowAddFocus")
            {
                AddFocusFlyout.IsOpen = true;
            }
            if (msg.Notification == "HideAddFocus")
            {
                AddFocusFlyout.IsOpen = false;
            }
            if (msg.Notification == "ShowEditFocus")
            {
                EditFocusFlyout.IsOpen = true;
                //((EditFocusViewModel)EditFocusFlyout.DataContext).Focus = (Model.Focus)msg.Sender;
            }
            if (msg.Notification == "HideEditFocus")
            {
                EditFocusFlyout.IsOpen = false;
            }
            if (msg.Notification == "ShowChangeImage")
            {
                ChangeImage view = new ChangeImage();
                view.Show();
            }
        }
    }
}
