using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.ExceptionServices;
using FocusTreeManager.Helper;
using System.IO;
using DiffPlex.DiffBuilder;
using DiffPlex;

namespace FocusTreeManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var diffBuilder = new SideBySideDiffBuilder(new Differ());
            //var model = diffBuilder.BuildDiffModel(
            //    File.ReadAllText(@"C:\users\stpigu01\desktop\usa.txt"),
            //    File.ReadAllText(@"C:\users\stpigu01\desktop\usa2.txt"));

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += HandleFirstChance;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleCrashes);
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
