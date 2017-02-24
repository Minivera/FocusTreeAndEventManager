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

        public static List<ImageSource> findAllGameImages(ImageType source)
        {
            List<ImageSource> list = new List<ImageSource>();
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
            string fullpath = Configurator.getGamePath() + rightFolder;
            //For each file in the normal folder
            foreach (string fileName in Directory.GetFiles(fullpath, "*" + GFX_EXTENTION, 
                                                           SearchOption.TopDirectoryOnly))
            {
                if (IMAGE_DO_NOT_LOAD.Any(fileName.Contains))
                {
                    continue;
                }
                try
                {
                    using (FileStream stream = new FileStream(fileName, FileMode.Open))
                    {
                        DDSImage image = new DDSImage(stream);
                        list.Add(ImageSourceForBitmap(image.BitmapImage));
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            //For each file in add mod folders
            ProjectModel model = (new ViewModelLocator()).Main.Project;
            foreach (string modpath in model.ListModFolders)
            {
                fullpath = modpath + rightFolder;
                foreach (string fileName in Directory.GetFiles(fullpath, "*" + GFX_EXTENTION,
                                                                   SearchOption.TopDirectoryOnly))
                {
                    if (IMAGE_DO_NOT_LOAD.Any(fileName.Contains))
                    {
                        continue;
                    }
                    try
                    {
                        using (FileStream stream = new FileStream(fileName, FileMode.Open))
                        {
                            DDSImage image = new DDSImage(stream);
                            list.Add(ImageSourceForBitmap(image.BitmapImage));
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                } 
            }
            return list;
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
