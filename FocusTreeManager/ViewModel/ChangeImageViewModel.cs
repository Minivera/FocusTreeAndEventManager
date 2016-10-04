using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChangeImageViewModel : ViewModelBase
    {
        const string IMAGE_PATH = @"GFX\Focus\";
        
        public RelayCommand<string> SelectCommand { get; private set; }

        private string focusImage;

        public string FocusImage
        {
            get
            {
                return focusImage;
            }
            set
            {
                focusImage = value;
                RaisePropertyChanged("FocusImage");
            }
        }

        public List<string> ImageList { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ChangeImageViewModel class.
        /// </summary>
        public ChangeImageViewModel()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), IMAGE_PATH);
            ImageList = new List<string>(Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly));

            SelectCommand = new RelayCommand<string>((s) => SelectFocus(s));
        }

        public void SelectFocus(string path)
        {
            FocusImage = Path.GetFileNameWithoutExtension(path);
            Messenger.Default.Send(new NotificationMessage(this, "HideChangeImage"));
        }
    }
}