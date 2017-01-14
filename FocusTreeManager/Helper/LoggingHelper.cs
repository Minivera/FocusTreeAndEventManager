using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            Writer writer = new Writer() { Filepath = LOG_FOLDER + logfile + ".log" };
            writer.WriteToFile(new StringBuilder(result));
        }
    }

    internal class Writer
    {
        public string Filepath { get; set; }
        private static object locker = new Object();

        public void WriteToFile(StringBuilder text)
        {
            lock (locker)
            {
                using (FileStream file = new FileStream(Filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(file, Encoding.Unicode))
                {
                    writer.Write(text.ToString());
                }
            }

        }
    }
}
