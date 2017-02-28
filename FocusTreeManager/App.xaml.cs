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
            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.FirstChanceException += HandleFirstChance;
            //currentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleCrashes);
        }

        static void HandleFirstChance(object source, FirstChanceExceptionEventArgs e)
        {
            LoggingHelper.LogException(e.Exception);
        }

        static void HandleCrashes(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingHelper.LogCrash((Exception)e.ExceptionObject);
        }
    }
}
