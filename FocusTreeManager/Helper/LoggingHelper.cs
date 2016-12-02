using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Helper
{
    class LoggingHelper
    {
        const string LOG_FOLDER = @"Log\";

        public static void LogException(Exception e)
        {
            WriteResult(e.ToString(), "Exceptions");
        }

        public static void LogCrash(Exception e)
        {
            WriteResult(e.ToString(), "Crashes");
        }

        private static void WriteResult(string result, string logfile)
        {
            using (StreamWriter sr = File.AppendText(LOG_FOLDER + logfile))
            {
                sr.WriteLine(result);
                sr.Flush();
            }
        }
    }
}
