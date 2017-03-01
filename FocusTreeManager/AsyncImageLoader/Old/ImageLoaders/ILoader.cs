using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FocusTreeManager.AsyncImageLoader.ImageLoaders
{
    public interface ILoader
    {
        Stream Load(string source);
    }
}
