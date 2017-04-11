using System;
using System.Windows;
using System.Runtime.ExceptionServices;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Helper;

namespace FocusTreeManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AsyncImageLoader.AsyncImageLoader.Worker.StartTheJob();
            //Logging
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += HandleFirstChance;
            currentDomain.UnhandledException += HandleCrashes;
        }

        private static void HandleFirstChance(object source, FirstChanceExceptionEventArgs e)
        {
            //Ignore syntax exceptions
            if (e.Exception is SyntaxException) return;
            LoggingHelper.LogException(e.Exception);
        }

        private static void HandleCrashes(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingHelper.LogCrash((Exception)e.ExceptionObject);
        }
    }
}
