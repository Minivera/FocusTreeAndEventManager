using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views.CodeEditor;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Scripter : UserControl
    {
        public static readonly DependencyProperty ScriptProperty =
        DependencyProperty.Register("Script", typeof(Script), typeof(UserControl),
        new PropertyMetadata(new Script(), OnScriptChanged));

        public static void OnScriptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scripter sender = d as Scripter;
            ScripterViewModel Vm = sender.DataContext as ScripterViewModel;
            if (Vm != null)
            {
                Vm.ManagedScript = e.NewValue as Script;
            }
        }

        public Script Script
        {
            get { return (Script)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value); }
        }

        public static readonly DependencyProperty ScripterTypeProperty =
        DependencyProperty.Register("CurrentType", typeof(ScripterType), typeof(UserControl),
        new PropertyMetadata(ScripterType.Generic, OnScriptTypeChanged));

        public static void OnScriptTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scripter sender = d as Scripter;
            ScripterViewModel Vm = sender.DataContext as ScripterViewModel;
            if (Vm != null)
            {
                Vm.ScriptType = e.NewValue != null ? (ScripterType)e.NewValue : ScripterType.Generic;
            }
        }

        public ScripterType CurrentType
        {
            get { return (ScripterType)GetValue(ScripterTypeProperty); }
            set { SetValue(ScripterTypeProperty, value); }
        }

        public Scripter()
        {
            InitializeComponent();
            loadLocales();
            DataContext = this;
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
            ScripterViewModel Vm = TabScript.DataContext as ScripterViewModel;
            if (Vm != null)
            {
                Vm.ManagedScript = Script;
                Vm.ScriptType = CurrentType;
            }
            if (Configurator.getScripterPreference() == "Scripter")
            {
                Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = ScripterTab));
                ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Scripter";
            }
            else if(Configurator.getScripterPreference() == "Editor")
            {
                Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = EditorTab));
                ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Editor";
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
                    ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Scripter";
                    ((ScripterViewModel)TabScript.DataContext).ScriptToScripter();
                }
                else if (selectedTab == EditorTab)
                {
                    ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Editor";
                    ((ScripterViewModel)TabScript.DataContext).ScripterToScript();
                }
            }
        }
    }
}
