using System;

namespace FocusTreeManager.Helper
{
    public class LoggingHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogException(Exception e)
        {
            log.Error("An error occurred during the application's runtime", e);
        }

        public static void LogCrash(Exception e)
        {
            log.Fatal("A fatal error occurred", e);
        }
    }
}
