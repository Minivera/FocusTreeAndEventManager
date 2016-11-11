using FocusTreeManager.CodeStructures;
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
using System.Windows.Shapes;
using static FocusTreeManager.ViewModel.ScripterControlsViewModel;

namespace FocusTreeManager.Views
{
    public partial class EditScript : MetroWindow
    {
        public Script ScriptText { get; private set; }

        public ScripterType ScriptType { get; private set; }

        public EditScript(Script ScriptText, ScripterType ScriptType)
        {
            InitializeComponent();
            loadLocales();
            this.ScriptText = ScriptText;
            this.ScriptType = ScriptType;
            DataContext = this;
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
            if (msg.Notification == "HideScripter")
            {
                this.Hide();
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
