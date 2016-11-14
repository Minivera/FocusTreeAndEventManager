using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        public class ImageData
        {
            public string Image { get; set; }
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
            int MaxWidth = SubFolder == "Focus" ? 100 : 250;
            ImageList.Clear();
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                GFX_FOLDER + SubFolder + "\\");
            foreach (string fileName in Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly))
            {
                ImageList.Add(new ImageData() {
                    Image = IMAGE_PATH + SubFolder + "/" + Path.GetFileName(fileName),
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
        }
    }
}