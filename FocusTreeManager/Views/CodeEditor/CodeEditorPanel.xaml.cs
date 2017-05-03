using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Views.CodeEditor
{
    public partial class CodeEditorPanel : UserControl
    {

        public bool ShowStruct
        {
            get
            {
                return Configurator.getEditorShowStruct();
            }
            set
            {
                Configurator.setEditorShowStruct(value);
            }
        }

        public bool ShowPlan
        {
            get
            {
                return Configurator.getEditorShowPlan();
            }
            set
            {
                Configurator.setEditorShowPlan(value);
            }
        }

        private Find FindPanel;

        private Replace ReplacePanel;

        public CodeEditorPanel()
        {
            InitializeComponent();
            Editor.RenderMethod = UserControlRendered;
            Editor.TextUpdated = UserControlTextUpdated;
            loadLocales();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Toggle Buttons
            if (!ShowStruct)
            {
                GridPreview.Visibility = Visibility.Hidden;
                ColumnStructure.Width = new GridLength(0);
                GridSplitter.Visibility = Visibility.Hidden;
                ShowStructButton.IsChecked = false;
            }
            else
            {
                GridPreview.Visibility = Visibility.Visible;
                ColumnStructure.Width = new GridLength(200);
                GridSplitter.Visibility = Visibility.Visible;
                ShowStructButton.IsChecked = true;
            }
            if (!ShowPlan)
            {
                GridNvaigation.Visibility = Visibility.Hidden;
                ColumnPreview.Width = new GridLength(0);
                ShowPlanButton.IsChecked = false;
            }
            else
            {
                GridNvaigation.Visibility = Visibility.Visible;
                ColumnPreview.Width = new GridLength(200);
                ShowPlanButton.IsChecked = true;
            }
            //Loaded Event
            Loaded += (s, e) =>
            {
                ThumbFind.ApplyTemplate();
                ThumbReplace.ApplyTemplate();
                FindPanel = (Find)ThumbFind.Template.FindName("FindPanel", ThumbFind);
                ReplacePanel = (Replace)ThumbReplace.Template.FindName("ReplacePanel", ThumbReplace);
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
            Resources.MergedDictionaries.Add(LocalizationHelper.getLocale());
        }

        private void UserControlRendered()
        {
            //Remove if editor already exists
            List<UIElement> itemstoremove = NavigatorGrid.Children.
                OfType<CodeNavigator>().Cast<UIElement>().ToList();
            foreach (UIElement children in itemstoremove)
            {
                NavigatorGrid.Children.Remove(children);
            }
            //Obtain and add navigator
            CodeNavigator navigator = Editor.GetNavigator();
            DockPanel.SetDock(navigator, Dock.Right);
            NavigatorGrid.Children.Add(navigator);
            //Setup the Code Viewer
            Viewer.SetupViewer(Editor.Text);
            Viewer.LinkedEditor = Editor;
            //Setup the find and replace dialogs
            FindPanel.LinkedEditor = Editor;
            ReplacePanel.LinkedEditor = Editor;
        }

        private ScriptErrorLogger UserControlTextUpdated(string Text)
        {
            //Setup the Code Viewer
            ScriptErrorLogger log = Viewer.SetupViewer(Editor.Text);
            Viewer.LinkedEditor = Editor;
            return log;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FindPanel.Visibility = Visibility.Visible;
            ReplacePanel.Visibility = Visibility.Hidden;
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            FindPanel.Visibility = Visibility.Hidden;
            ReplacePanel.Visibility = Visibility.Visible;
        }

        private void ShowStructButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowStructButton.IsChecked == null) return;
            ShowStruct = (bool)ShowStructButton.IsChecked;
            if (!(bool)ShowStructButton.IsChecked)
            {
                GridPreview.Visibility = Visibility.Hidden;
                ColumnStructure.Width = new GridLength(0);
                GridSplitter.Visibility = Visibility.Hidden;
            }
            else
            {
                GridPreview.Visibility = Visibility.Visible;
                ColumnStructure.Width = new GridLength(200);
                GridSplitter.Visibility = Visibility.Visible;
            }
        }

        private void ShowPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowPlanButton.IsChecked == null) return;
            ShowPlan = (bool)ShowPlanButton.IsChecked;
            if (!(bool)ShowPlanButton.IsChecked)
            {
                GridNvaigation.Visibility = Visibility.Hidden;
                ColumnPreview.Width = new GridLength(0);
            }
            else
            {
                GridNvaigation.Visibility = Visibility.Visible;
                ColumnPreview.Width = new GridLength(200);
            }
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;
            if (thumb == null) return;
            Canvas.SetRight(thumb, Canvas.GetRight(thumb) - e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }
    }
}
