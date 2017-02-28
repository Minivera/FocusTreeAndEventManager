using FocusTreeManager.AsyncImageLoader;
using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FocusTreeManager.Helper
{
    public enum ImageType
    {
        Goal,
        Event
    }

    static class ImageHelper
    {
        private const string GFX_GOAL_FOLDER = @"\gfx\interface\goals\";

        private const string GFX_EVENT_FOLDER = @"\gfx\interface\goals\";

        private const string GFX_EXTENTION = ".dds";

        private static readonly string[] IMAGE_DO_NOT_LOAD = { "shine_mask", "shine_overlay"};

        public static ImageSource getImageFromGame(string imageName, ImageType source)
        {
            string rightFolder = "";
            switch (source)
            {
                case ImageType.Goal:
                    rightFolder = GFX_GOAL_FOLDER;
                    break;
                case ImageType.Event:
                    rightFolder = GFX_EVENT_FOLDER;
                    break;
            }
            //Build the error image if the requested one do not exist
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("PhotoLoader;component/Resources.xaml", UriKind.Relative);
            DrawingImage errorThumbnail = resourceDictionary["ImageError"] as DrawingImage;
            errorThumbnail.Freeze();
            //Try to obtain the image from the game's folder
            string fullpath = Configurator.getGamePath() + rightFolder + imageName + GFX_EXTENTION;
            if (File.Exists(fullpath))
            {
                try
                {
                    using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                    {
                        DDSImage image = new DDSImage(stream);
                        return ImageSourceForBitmap(image.BitmapImage);
                    }
                }
                catch (Exception)
                {
                    return errorThumbnail;
                }
            }
            else
            {
                //Load from mod folders
                ProjectModel model = (new ViewModelLocator()).Main.Project;
                foreach (string modpath in model.ListModFolders)
                {
                    fullpath = modpath + rightFolder + imageName + GFX_EXTENTION;
                    try
                    {
                        using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                        {
                            DDSImage image = new DDSImage(stream);
                            return ImageSourceForBitmap(image.BitmapImage);
                        }
                    }
                    catch (Exception)
                    {
                        return errorThumbnail;
                    }
                }
            }
            return errorThumbnail;
        }

        public static Dictionary<string, ImageSource> findAllGameImages(ImageType source)
        {
            Dictionary<string, ImageSource> dictionary = new Dictionary<string, ImageSource>();
            string str = "";
            if (source != ImageType.Goal)
            {
                if (source == ImageType.Event)
                {
                    str = @"\gfx\interface\goals\";
                }
            }
            else
            {
                str = @"\gfx\interface\goals\";
            }
            foreach (string str2 in Directory.GetFiles(Configurator.getGamePath() + 
                str, "*.dds", SearchOption.TopDirectoryOnly))
            {
                if (!IMAGE_DO_NOT_LOAD.Any<string>(new Func<string, bool>(str2.Contains)))
                {
                    try
                    {
                        using (FileStream stream = new FileStream(str2, FileMode.Open))
                        {
                            DDSImage image = new DDSImage(stream);
                            dictionary[str2] = ImageSourceForBitmap(image.BitmapImage);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            using (IEnumerator<string> enumerator = new ViewModelLocator().Main.
                Project.ListModFolders.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (string str3 in Directory.GetFiles(enumerator.Current + str, "*.dds", SearchOption.TopDirectoryOnly))
                    {
                        if (!IMAGE_DO_NOT_LOAD.Any<string>(new Func<string, bool>(str3.Contains)))
                        {
                            try
                            {
                                using (FileStream stream2 = new FileStream(str3, FileMode.Open))
                                {
                                    DDSImage image2 = new DDSImage(stream2);
                                    dictionary[str3] = ImageSourceForBitmap(image2.BitmapImage);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private static ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, 
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
