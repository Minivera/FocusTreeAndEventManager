using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using FocusTreeManager.Helper;

namespace FocusTreeManager.AsyncImageLoader
{
    public class AsyncImageLoader
    {
        private readonly BackgroundWorker StarterWorker;

        private readonly BackgroundWorker ModWorker;

        private bool restartStarterWorker;

        private static readonly object myLock = new object();

        private static AsyncImageLoader backWorkerSingleton = new AsyncImageLoader();

        public Dictionary<string, ImageSource> Focuses { get; set; }

        public Dictionary<string, ImageSource> Events { get; set; }

        private AsyncImageLoader()
        {
            StarterWorker = new BackgroundWorker();
            StarterWorker.WorkerSupportsCancellation = true;
            StarterWorker.DoWork += StarterWorker_DoWork;
            StarterWorker.RunWorkerCompleted += StarterWorker_RunWorkerCompleted;
            ModWorker = new BackgroundWorker();
            ModWorker.DoWork += ModWorker_DoWork;
        }

        private void StarterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (restartStarterWorker)
            {
                restartStarterWorker = false;
                StartTheJob();
            }
        }

        private void StarterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Focuses = ImageHelper.findAllGameImages(ImageType.Goal);
            Events = ImageHelper.findAllGameImages(ImageType.Event);
        }

        private void ModWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (KeyValuePair<string, ImageSource> images
                in ImageHelper.RefreshFromMods(ImageType.Goal))
            {
                Focuses[images.Key] = images.Value;
            }
            foreach (KeyValuePair<string, ImageSource> images
                in ImageHelper.RefreshFromMods(ImageType.Event))
            {
                Events[images.Key] = images.Value;
            }
        }

        public void RestartStarterWorker()
        {
            if (!StarterWorker.IsBusy)
            {
                StartTheJob();
                return;
            }

            restartStarterWorker = true;
            StarterWorker.CancelAsync();
        }

        public void StartTheJob()
        {
            StarterWorker.RunWorkerAsync();
        }

        public void RefreshFromMods()
        {
            ModWorker.RunWorkerAsync();
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