using System;

namespace ISIC_Concole_Downloader
{
    /// <summary>
    /// Basic logger class to log messages and erros to file and print them on the console
    /// </summary>
    class Logger
    {
        public static void LogMsg(string msg)
        {
            string filename = @"Log-" + DateTime.Now.ToShortDateString().Replace(@"/", @"-") + @".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename, true))
            {
                file.WriteLine(DateTime.Now + @": " + msg);
            }
            Console.WriteLine(DateTime.Now + @": " + msg);
        }

        public static void LogErr(string err)
        {
            string filename = @"Log-" + DateTime.Now.ToShortDateString().Replace(@"/", @"-") + @".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename, true))
            {
                file.WriteLine(DateTime.Now + @" ERROR: " + err);
            }
            Console.WriteLine(DateTime.Now + @" ERROR: " + err);
        }
    }
}
