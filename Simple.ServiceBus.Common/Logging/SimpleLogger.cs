using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Logging
{
    public class SimpleLogger
    {
        protected static object _thatObj = new object();
        protected static readonly string _baseDirectory;

        static SimpleLogger()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        internal static void Write(string text, string fileName)
        {
            lock (_thatObj)
            {
                var nowPrefix = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>";
                try
                {
                    FileInfo fin = new FileInfo(fileName);
                    using (StreamWriter sw = fin.AppendText())
                    {

                        sw.Write(nowPrefix);
                        sw.Write(text + System.Environment.NewLine);
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} file:{1}, content:{2}, write log exception:{3}", nowPrefix, fileName, text, ex);
                }
            }
        }

        public static void Info(string text)
        {
            string logFileName = Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyyMMdd") + ".log");

            Write(text, logFileName);
        }
    }
}
