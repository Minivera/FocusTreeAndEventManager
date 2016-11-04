using FocusTreeManager.ViewModel;
using FocusTreeManager.Views.CodeEditor;
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

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Logique d'interaction pour Scripter.xaml
    /// </summary>
    public partial class Scripter : UserControl
    {
        public Scripter()
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
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Configurator.getScripterPreference() == "Scripter")
            {
                Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = ScripterTab));
                ((ScripterViewModel)DataContext).SelectedTabIndex = "Scripter";
            }
            else if(Configurator.getScripterPreference() == "Editor")
            {
                Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = EditorTab));
                ((ScripterViewModel)DataContext).SelectedTabIndex = "Editor";
            }
            TabScript.SelectionChanged += new SelectionChangedEventHandler(TabScript_SelectionChanged);
        }

        private void TabScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                TabItem selectedTab = e.AddedItems[0] as TabItem;
                if (selectedTab == ScripterTab)
                {
                    ((ScripterViewModel)DataContext).SelectedTabIndex = "Scripter";
                    ((ScripterViewModel)DataContext).ScriptToScripter();
                }
                else if (selectedTab == EditorTab)
                {
                    ((ScripterViewModel)DataContext).SelectedTabIndex = "Editor";
                    ((ScripterViewModel)DataContext).ScripterToScript();
                }
            }
        }
    }
}
