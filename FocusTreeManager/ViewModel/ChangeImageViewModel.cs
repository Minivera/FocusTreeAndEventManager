using FocusTreeManager.Helper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;

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
        public class ImageData
        {
            public ImageSource Image { get; set; }
            public string Filename { get; set; }
            public int MaxWidth { get; set; }
        }

        const string GFX_FOLDER = @"GFX\";

        const string IMAGE_PATH = @"pack://application:,,,/FocusTreeManager;component/GFX/";
        
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

        public ObservableCollection<ImageData> ImageList { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ChangeImageViewModel class.
        /// </summary>
        public ChangeImageViewModel()
        {
            ImageList = new ObservableCollection<ImageData>();
            SelectCommand = new RelayCommand<string>((s) => SelectFocus(s));
        }

        public void LoadImages(string SubFolder, string CurrentImage)
        {
            int MaxWidth = 250;
            ImageType type = ImageType.Event;
            switch (SubFolder)
            {
                case "Focus":
                    MaxWidth = 100;
                    type = ImageType.Goal;
                    break;
                case "Event":
                    MaxWidth = 250;
                    type = ImageType.Event;
                    break;
            }
            
            ImageList.Clear();
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                GFX_FOLDER + SubFolder + "\\");
            foreach (KeyValuePair<string, ImageSource> source in ImageHelper.findAllGameImages(type))
            {
                ImageList.Add(new ImageData() {
                    Image = source.Value,
                    Filename = source.Key,
                    MaxWidth = MaxWidth
                });
            }
            focusImage = CurrentImage;
            RaisePropertyChanged(() => ImageList);
        }

        public void SelectFocus(string path)
        {
            FocusImage = Path.GetFileNameWithoutExtension(path);
            Messenger.Default.Send(new NotificationMessage(this, "HideChangeImage"));
            Close();
        }

        private void Close()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }
    }
}