using System;
using System.IO;

namespace CoolFishNS.Utilities
{
    internal class Log
    {

        private static string _fileNameDate;
        private static readonly object LockObject = new object();

        internal Log()
        {
            _fileNameDate = DateTime.Now.ToShortDateString().Replace("/", "-") + " " + TimeStamp.Replace(":", ".");
            if (!Directory.Exists(Utilities.ApplicationPath + "\\Logs"))
            {
                Directory.CreateDirectory(Utilities.ApplicationPath + "\\Logs");
            }
            Logging.OnLog += LogMessage;
            Logging.OnWrite += LogMessage;
        }

        internal static string TimeStamp
        {
            get { return string.Format("[{0}] ", DateTime.Now.ToLongTimeString()); }
        }

        internal static void LogMessage(object sender, MessageEventArgs messageEventArgs)
        {
            lock (LockObject)
            {
                using (TextWriter tw =
               new StreamWriter(
                   String.Format("{0}\\Logs\\[CoolFish] {1} Log.txt", Utilities.ApplicationPath, _fileNameDate),
                   true))
                {
                    tw.WriteLine(TimeStamp + " " + messageEventArgs.Message);
                }
            }
            
        }
    }
}