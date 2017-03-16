using FocusTreeManager.AsyncImageLoader;
using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

    public static class ImageHelper
    {
        private const string GFX_GOAL_FOLDER = @"\gfx\interface\goals\";

        private const string GFX_EVENT_FOLDER = @"\gfx\event_pictures";

        private const string GFX_EXTENTION = ".dds";

        private static readonly string[] IMAGE_DO_NOT_LOAD = { "shine_mask", "shine_overlay"};

        public static ImageSource getImageFromGame(string imageName, ImageType source)
        {
            //If we couldn't find the error image, throw an IO exception
            //TODO: Prepare an error image
            //Try to obtain the image from the game's folder
            try
            {
                switch (source)
                {
                    case ImageType.Goal:
                        return AsyncImageLoader.AsyncImageLoader.Worker.
                            Focuses.FirstOrDefault(f => Path.
                            GetFileNameWithoutExtension(f.Key) == imageName).Value;
                    case ImageType.Event:
                        return AsyncImageLoader.AsyncImageLoader.Worker.
                            Events.FirstOrDefault(f => Path.
                            GetFileNameWithoutExtension(f.Key) == imageName).Value;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), source, null);
                }
            }
            catch (Exception)
            {
                //If an error occurred, return the error image
                //TODO: Return something
                return null;
            }
        }

        public static Dictionary<string, ImageSource> findAllGameImages(ImageType source)
        {
            Dictionary<string, ImageSource> list = new Dictionary<string, ImageSource>();
            string rightFolder;
            switch (source)
            {
                case ImageType.Goal:
                    rightFolder = GFX_GOAL_FOLDER;
                    break;
                case ImageType.Event:
                    rightFolder = GFX_EVENT_FOLDER;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
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
                        ImageSource result = ImageSourceForBitmap(image.BitmapImage);
                        result.Freeze();
                        list[fileName] = result;
                    }
                }
                catch (Exception)
                {
                    // ignored, we don't want to kill the whole process for one missing image
                }
            }
            //For each file in add mod folders
            //ProjectModel model = new ViewModelLocator().Main.Project;
            //if (model?.ListModFolders == null) return list;
            //{
            //    foreach (string modpath in model.ListModFolders)
            //    {
            //        fullpath = modpath + rightFolder;
            //        foreach (string fileName in Directory.GetFiles(fullpath, "*" + GFX_EXTENTION,
            //            SearchOption.TopDirectoryOnly))
            //        {
            //            if (IMAGE_DO_NOT_LOAD.Any(fileName.Contains))
            //            {
            //                continue;
            //            }
            //            try
            //            {
            //                using (FileStream stream = new FileStream(fileName, FileMode.Open))
            //                {
            //                    DDSImage image = new DDSImage(stream);
            //                    list[fileName] = (ImageSourceForBitmap(image.BitmapImage));
            //                }
            //            }
            //            catch (Exception)
            //            {
            //                // ignored, we don't want to kill the whole process for one missing image
            //            }
            //        } 
            //    }
            //}
            return list;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private static ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, 
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
