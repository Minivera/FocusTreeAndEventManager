using System;
using System.CodeDom;
using System.Windows;
using System.Runtime.ExceptionServices;
using FocusTreeManager.Helper;

namespace FocusTreeManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Log only if not in debug
        #if DEBUG
            public bool toLog = false;
        #else
            public bool toLog = true;
        #endif

        public App()
        {
            AsyncImageLoader.AsyncImageLoader.Worker.StartTheJob();
            //Logging
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += HandleFirstChance;
            currentDomain.UnhandledException += HandleCrashes;
        }

        private void HandleFirstChance(object source, FirstChanceExceptionEventArgs e)
        {
            if (!toLog) return;
            LoggingHelper.LogException(e.Exception);
        }

        private  void HandleCrashes(object sender, UnhandledExceptionEventArgs e)
        {
            if (!toLog) return;
            LoggingHelper.LogCrash((Exception)e.ExceptionObject);
        }
    }
}
