using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using FocusTreeManager.Helper;

namespace FocusTreeManager.AsyncImageLoader
{
    public class AsyncImageLoader
    {
        private readonly BackgroundWorker backgroundWorker;

        private static readonly object myLock = new object();

        private static AsyncImageLoader backWorkerSingleton = new AsyncImageLoader();

        public Dictionary<string, ImageSource> Focuses { get; set; }

        public Dictionary<string, ImageSource> Events { get; set; }

        private AsyncImageLoader()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Focuses = ImageHelper.findAllGameImages(ImageType.Goal);
            Events = ImageHelper.findAllGameImages(ImageType.Event);
        }

        public void StartTheJob()
        {
            backgroundWorker.RunWorkerAsync();
        }

        public static AsyncImageLoader Worker
        {
            get
            {
                lock (myLock)
                {
                    if (backWorkerSingleton == null)
                    {
                        backWorkerSingleton = new AsyncImageLoader();
                    }
                }
                return backWorkerSingleton;
            }
        }
    }
}