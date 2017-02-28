using System;
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
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>(App.HandleFirstChance);
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.HandleCrashes);
        }

        private static void HandleCrashes(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingHelper.LogCrash((Exception)e.ExceptionObject);
        }

        private static void HandleFirstChance(object source, FirstChanceExceptionEventArgs e)
        {
            LoggingHelper.LogException(e.Exception);
        }
    }
}
