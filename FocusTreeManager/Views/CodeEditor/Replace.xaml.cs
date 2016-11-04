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
    public partial class Replace : UserControl, INotifyPropertyChanged
    {
        const int MAX_HISTORY = 15;

        private FixedSizeQueue<string> historyFind = new FixedSizeQueue<string>(MAX_HISTORY);

        private FixedSizeQueue<string> historyReplace = new FixedSizeQueue<string>(MAX_HISTORY);

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

        public ObservableCollection<string> ReplaceHistory
        {
            get
            {
                return new ObservableCollection<string>(historyReplace.ToList());
            }
        }

        public CodeEditor LinkedEditor { get; set; }

        public Replace()
        {
            InitializeComponent();
            loadLocales();
            //Messenger
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

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if something is searched
            if (string.IsNullOrEmpty(LastSearch))
            {
                //If not, run the find button beforehand
                NextButton_Click(null, null);
            }
            MatchCollection results = LinkedEditor.Replace(new Regex(LastSearch), ComboReplace.Text, CurrentIndex);
            FindSize = results.Count;
            if (!ReplaceHistory.Contains(ComboReplace.Text))
            {
                historyReplace.Enqueue(ComboReplace.Text);
                OnPropertyChanged("ReplaceHistory");
            }
            NextButton_Click(null, null);
            LinkedEditor.Focus();
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            NextButton_Click(null, null);
            for (int i = 0; i < FindSize; i++)
            {
                LinkedEditor.Replace(new Regex(LastSearch), ComboReplace.Text, 0);
            }
            LastSearch = null;
            CurrentIndex = 0;
            FindSize = 0;
            LinkedEditor.EndFindAndReplace();
            if (!ReplaceHistory.Contains(ComboReplace.Text))
            {
                historyReplace.Enqueue(ComboReplace.Text);
                OnPropertyChanged("ReplaceHistory");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LastSearch = null;
            CurrentIndex = 0;
            FindSize = 0;
            Visibility = Visibility.Hidden;
            LinkedEditor.EndFindAndReplace();
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
