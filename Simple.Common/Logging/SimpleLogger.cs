using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simple.Common.Logging
{
    public class SimpleLogger
    {
        protected static object _thatObj = new object();
        protected static readonly string _baseDirectory;

        protected static SimpleLogControl LogControl = new SimpleLogControl();

        static SimpleLogger()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        internal static void Write(string text, string fileName)
        {
            lock (_thatObj)
            {
                string nowPrefix = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>";
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    using (StreamWriter writer = fileInfo.AppendText())
                    {
                        writer.Write(nowPrefix);
                        writer.Write(text + System.Environment.NewLine);
                        writer.Close();
                    }

                    LogControl.CheckFile(fileInfo.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}file:{1}, content:{2}, write log exception:{3}", nowPrefix, fileName, text, ex);
                }
            }
        }

        public static void Info(string text)
        {
            string logFileName = Path.Combine(_baseDirectory, LogControl.FileName);

            Write(text, logFileName);
        }

        protected class SimpleLogControl
        {
            public DateTime Now { get; protected set; }

            public bool IsCreateNewFile { get; protected set; }

            public Int64 LogSize { get; set; }

            public string FileName { get; protected set; }

            public SimpleLogControl()
            {
                Now = DateTime.Now;

                LogSize = 20971520;//20MB

                FileName = GetFileName("yyyyMMdd");
            }

            private string GetFileName(string timeFormat)
            {
                return DateTime.Now.ToString(timeFormat) + ".log";
            }

            public void CheckFile(Int64 fileLength)
            {
                if (Now.Date != DateTime.Now.Date)
                {
                    Now = DateTime.Now;
                    FileName = GetFileName("yyyyMMdd");

                    return;
                }

                IsCreateNewFile = fileLength >= LogSize;

                if (IsCreateNewFile)
                {
                    FileName = GetFileName("yyyyMMdd_HHmmss");
                }
            }
        }
    }
}
