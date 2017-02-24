using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Resources;

namespace FocusTreeManager.AsyncImageLoader.ImageLoaders
{
    public class LocalDiskLoader: ILoader
    {

        public Stream Load(string source)
        {
            try
            {
                return Application.GetResourceStream(
                            new Uri(source, UriKind.RelativeOrAbsolute)).Stream;
            }
            catch
            {
                return File.OpenRead(source);
            }
        }
    }
}
