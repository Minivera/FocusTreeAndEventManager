using FocusTreeManager.Containers;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FocusTreeManager.Views.CodeEditor
{
    public partial class Find : UserControl, INotifyPropertyChanged
    {
        const int MAX_HISTORY = 15;

        private FixedSizeQueue<string> historyFind = new FixedSizeQueue<string>(MAX_HISTORY);

        private int CurrentIndex = 0;

        private int FindSize = 0;

        private string LastSearch;

        public ObservableCollection<string> SearchHistory
        {
            get
            {
                return new ObservableCollection<string>(historyFind.ToList());
            }
        }

        public CodeEditor LinkedEditor { get; set; }

        public Find()
        {
            InitializeComponent();
            loadLocales();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Events
            //MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            //MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            //MouseMove += new MouseEventHandler(Control_MouseMove);
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

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //Build regex
            string regexText = "";
            if (CaseCheck.IsChecked != null && (bool)CaseCheck.IsChecked)
            {
                regexText += "(?i)";
            }
            if (FullWordCheck.IsChecked != null && (bool)FullWordCheck.IsChecked)
            {
                regexText += @"\b" + ComboFind.Text +  @"\b";
            }
            else
            {
                regexText += ComboFind.Text;
            }
            //If it is the first time this is searched
            if (string.IsNullOrEmpty(LastSearch) || regexText != LastSearch)
            {
                OccurencesLabel.Visibility = Visibility.Hidden;
                CountLabel.Visibility = Visibility.Hidden;
                LinkedEditor.EndFindAndReplace();
                CurrentIndex = 0;
                LastSearch = regexText;
            }
            else
            {
                CurrentIndex = CurrentIndex + 1 < FindSize ? CurrentIndex + 1 : 0;
            }
            MatchCollection results = LinkedEditor.Find(new Regex(regexText), CurrentIndex);
            FindSize = results.Count;
            if (!SearchHistory.Contains(ComboFind.Text))
            {
                historyFind.Enqueue(ComboFind.Text);
                OnPropertyChanged("SearchHistory");
            }
            LinkedEditor.Focus();
        }

        private void CountButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if something is searched
            if (string.IsNullOrEmpty(LastSearch))
            {
                //If not, run the find button beforehand
                NextButton_Click(null, null);
            }
            //Show the count
            OccurencesLabel.Visibility = Visibility.Visible;
            CountLabel.Visibility = Visibility.Visible;
            CountLabel.Content = FindSize;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LastSearch = null;
            CurrentIndex = 0;
            FindSize = 0;
            Visibility = Visibility.Hidden;
            LinkedEditor.EndFindAndReplace();
        }

        //private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = true;
        //    var draggableControl = sender as UserControl;
        //    clickPosition = e.GetPosition((FrameworkElement)this.Parent);
        //    draggableControl.CaptureMouse();
        //}

        //private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = false;
        //    var draggable = sender as UserControl;
        //    draggable.ReleaseMouseCapture();
        //}

        //private void Control_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var draggableControl = sender as UserControl;
        //    if (isDragging && draggableControl != null)
        //    {
        //        Point currentPosition = e.GetPosition(this.Parent as UIElement);
        //        var transform = draggableControl.RenderTransform as TranslateTransform;
        //        if (transform == null)
        //        {
        //            transform = new TranslateTransform();
        //            draggableControl.RenderTransform = transform;
        //        }
        //        transform.X = currentPosition.X - clickPosition.X;
        //        transform.Y = currentPosition.Y - clickPosition.Y;
        //    }
        //}

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
