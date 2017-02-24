using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FocusTreeManager.AsyncImageLoader.ImageLoaders
{
    public static class LoaderFactory
    {
        public static ILoader CreateLoader(SourceType sourceType)
        {
            switch (sourceType)
            {
                case SourceType.LocalDisk:
                    return new LocalDiskLoader();
                default:
                    throw new ApplicationException("Unexpected exception");
            }
        }
    }
}
