using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using Imaging.DDSReader;

namespace FocusTreeManager.Helper
{

    public enum ImageType
    {
        Goal,
        Event
    }

    public static class ImageHelper
    {
        private static readonly string[] ARRAY_FILE_NAME =
        {
            "goal_support_democracy",
            "goal_support_fascism",
            "goal_support_communism" ,
            "goal_generic_occupy_states_coastal",
            "goal_generic_occupy_start_war",
            "goal_generic_occupy_states_ongoing_war",
            "goal_generic_build_navy"
        };

        private static readonly string[] ARRAY_ASSOCIATED_TYPO =
        {
            "goal_generic_support_democracy",
            "goal_generic_support_fascism",
            "goal_generic_support_communism",
            "goal_generic_occypy_states_coastal",
            "goal_generic_occypy_start_war",
            "goal_generic_occypy_states_ongoing_war",
            "goal_generic_build_nay"
        };

        private const string GFX_GOAL_FOLDER = @"\gfx\interface\goals\";

        private const string GFX_EVENT_FOLDER = @"\gfx\event_pictures";

        private const string GFX_EXTENTION = ".dds";

        private const string GFX_ERROR_IMAGE = @"\GFX\Focus\goal_unknown.png";

        private static readonly string[] IMAGE_DO_NOT_LOAD = { "shine_mask", "shine_overlay"};

        public static ImageSource getImageFromGame(string imageName, ImageType source)
        {
            //If we couldn't find the error image, throw an IO exception
            ImageSource errorSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() 
                + GFX_ERROR_IMAGE));
            //Try to obtain the image
            try
            {
                ImageSource value;
                switch (source)
                {
                    case ImageType.Goal:
                        value =  AsyncImageLoader.AsyncImageLoader.Worker.
                            Focuses.LastOrDefault(f => f.Key == imageName).Value;
                        break;
                    case ImageType.Event:
                        value =  AsyncImageLoader.AsyncImageLoader.Worker.
                            Events.LastOrDefault(f => f.Key == imageName).Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), source, null);
                }
                //Make sure the value is set, if not, return the error image.
                return value ?? errorSource;
            }
            catch (Exception)
            {
                //If an error occurred, return the error image
                return errorSource;
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
            if (!Directory.Exists(fullpath)) return list;
            //For each file in the normal folder
            foreach (string filename in Directory.GetFiles(fullpath, "*" + GFX_EXTENTION, 
                                                           SearchOption.TopDirectoryOnly))
            {
                if (IMAGE_DO_NOT_LOAD.Any(filename.Contains))
                {
                    continue;
                }
                try
                {
                    string imageName = Path.GetFileNameWithoutExtension(filename);
                    //try to replace potential broken links because of typos in the file names.
                    imageName = Array.IndexOf(ARRAY_ASSOCIATED_TYPO, imageName) != -1
                        ? ARRAY_FILE_NAME[Array.IndexOf(ARRAY_ASSOCIATED_TYPO, imageName)]
                        : imageName;
                    ImageSource result = ImageSourceForBitmap(DDS.LoadImage(filename));
                    result.Freeze();
                    list[imageName] = result;
                }
                catch (Exception)
                {
                    // ignored, we don't want to kill the whole process for one missing image
                }
            }
            return list;
        }

        public static Dictionary<string, ImageSource> RefreshFromMods(ImageType source)
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
            //For each file in add mod folders
            ProjectModel model = new ViewModelLocator().Main.Project;
            if (model?.ListModFolders == null) return list;
            {
                foreach (string modpath in model.ListModFolders)
                {
                    string fullpath = modpath + rightFolder;
                    if (!Directory.Exists(fullpath))
                    {
                        continue;
                    }
                    foreach (string filename in Directory.GetFiles(fullpath, "*" + GFX_EXTENTION,
                        SearchOption.TopDirectoryOnly))
                    {
                        if (IMAGE_DO_NOT_LOAD.Any(filename.Contains))
                        {
                            continue;
                        }
                        try
                        {
                            string imageName = Path.GetFileNameWithoutExtension(filename);
                            //try to replace potential broken links because of typos in the file names.
                            imageName = Array.IndexOf(ARRAY_ASSOCIATED_TYPO, imageName) != -1
                                ? ARRAY_FILE_NAME[Array.IndexOf(ARRAY_ASSOCIATED_TYPO, imageName)]
                                : imageName;
                            ImageSource result = ImageSourceForBitmap(DDS.LoadImage(filename));
                            result.Freeze();
                            list[imageName] = result;
                        }
                        catch (Exception)
                        {
                            // ignored, we don't want to kill the whole process for one missing image
                        }
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
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, 
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
