using FocusTreeManager.CodeStructures;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Windows;
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
                Hide();
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
