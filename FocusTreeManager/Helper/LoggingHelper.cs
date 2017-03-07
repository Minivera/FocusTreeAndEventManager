using System;
using System.Diagnostics;
using System.IO;

namespace FocusTreeManager.Helper
{
    public class LoggingHelper
    {
        private const string LOG_FOLDER = @"Log\";

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
            ConcurrentFile.WriteAllText(LOG_FOLDER + logfile + ".log", "\n\n" + result);
        }
    }

    internal static class ConcurrentFile
    {

        public static void WriteAllText(string path, string contents, int timeoutMs = 1000,
            FileMode mode = FileMode.Append)
        {
            using (FileStream stream = GetStream(path, mode, FileAccess.Write, timeoutMs))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(contents);
            }
        }

        private static FileStream GetStream(string path, FileMode mode, FileAccess access, int timeoutMs = 1000)
        {
            Stopwatch time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    return new FileStream(path, mode, access);
                }
                catch (IOException e)
                {
                    // access error
                    if (e.HResult != -2147024864)
                    {
                        throw;
                    }
                }
            }
            throw new TimeoutException($"Failed to get a access to {path} within {timeoutMs}ms.");
        }

        /// <summary>Deletes te file if it exists</summary>
        /// <returns>True if the file was deleted</returns>
        public static bool Delete(string filename, int timeoutMs = 1000)
        {
            Stopwatch time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    if (!File.Exists(filename))
                    {
                        return false;
                    }
                    File.Delete(filename);
                    return true;
                }
                catch (IOException e)
                {
                    // access error
                    if (e.HResult != -2147024864)
                    {
                        throw;
                    }
                }
            }
            throw new TimeoutException($"Failed to get a access to {filename} within {timeoutMs}ms.");
        }
    }
}
