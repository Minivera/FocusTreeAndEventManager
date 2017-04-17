using FocusTreeManager.Containers;
using FocusTreeManager.Helper;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FocusTreeManager.Views.CodeEditor
{
    public partial class Replace : UserControl, INotifyPropertyChanged
    {
        const int MAX_HISTORY = 15;

        private readonly FixedSizeQueue<string> historyFind = 
            new FixedSizeQueue<string>(MAX_HISTORY);

        private readonly FixedSizeQueue<string> historyReplace = 
            new FixedSizeQueue<string>(MAX_HISTORY);

        private int CurrentIndex;

        private int FindSize;

        private string LastSearch;

        public ObservableCollection<string> SearchHistory => 
            new ObservableCollection<string>(historyFind.ToList());

        public ObservableCollection<string> ReplaceHistory => 
            new ObservableCollection<string>(historyReplace.ToList());

        public CodeEditor LinkedEditor { get; set; }

        public Replace()
        {
            InitializeComponent();
            loadLocales();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            DataContext = this;
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
            if (ReplaceHistory.Contains(ComboReplace.Text)) return;
            historyReplace.Enqueue(ComboReplace.Text);
            OnPropertyChanged("ReplaceHistory");
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
