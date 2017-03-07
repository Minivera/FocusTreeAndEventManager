using System;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    public partial class Scripter : UserControl
    {
        public static readonly DependencyProperty ScriptProperty =
        DependencyProperty.Register("Script", typeof(Script), typeof(UserControl),
        new PropertyMetadata(new Script(), OnScriptChanged));

        public static void OnScriptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scripter sender = d as Scripter;
            ScripterViewModel Vm = sender?.DataContext as ScripterViewModel;
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
        DependencyProperty.Register("CurrentType", typeof(ScripterControlsViewModel.ScripterType), typeof(UserControl),
        new PropertyMetadata(ScripterControlsViewModel.ScripterType.Generic, OnScriptTypeChanged));

        public static void OnScriptTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scripter sender = d as Scripter;
            ScripterViewModel Vm = sender?.DataContext as ScripterViewModel;
            if (Vm != null)
            {
                Vm.ScriptType = (ScripterControlsViewModel.ScripterType?) e.NewValue ?? 
                    ScripterControlsViewModel.ScripterType.Generic;
            }
        }

        public ScripterControlsViewModel.ScripterType CurrentType
        {
            get { return (ScripterControlsViewModel.ScripterType)GetValue(ScripterTypeProperty); }
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
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ScripterViewModel Vm = TabScript.DataContext as ScripterViewModel;
            if (Vm != null)
            {
                Vm.ManagedScript = Script;
                Vm.ScriptType = CurrentType;
            }
            switch (Configurator.getScripterPreference())
            {
                case "Scripter":
                    Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = ScripterTab));
                    ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Scripter";
                    break;
                case "Editor":
                    Dispatcher.BeginInvoke((Action)(() => TabScript.SelectedItem = EditorTab));
                    ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Editor";
                    break;
            }
            TabScript.SelectionChanged += TabScript_SelectionChanged;
        }

        private void TabScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            TabItem selectedTab = e.AddedItems[0] as TabItem;
            if (Equals(selectedTab, ScripterTab))
            {
                ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Scripter";
                ((ScripterViewModel)TabScript.DataContext).ScriptToScripter();
            }
            else if (Equals(selectedTab, EditorTab))
            {
                ((ScripterViewModel)TabScript.DataContext).SelectedTabIndex = "Editor";
                ((ScripterViewModel)TabScript.DataContext).ScripterToScript();
            }
        }
    }
}
