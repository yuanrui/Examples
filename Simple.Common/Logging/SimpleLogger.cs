using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Simple.Common.Logging
{
    public class SimpleLogger
    {
        private const string TEXT_PREFIX_FORMAT = "yyyy-MM-dd HH:mm:ss>>";
        protected static Object _thatObj;
        protected static SimpleLogControl LogControl;

        static SimpleLogger()
        {
            _thatObj = new Object();
            LogControl = new SimpleLogControl();
            LogControl.Init();
        }

        internal static void Write(String text, String fileName)
        {
            lock (_thatObj)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    using (StreamWriter writer = fileInfo.AppendText())
                    {
                        writer.WriteLine(text);
                        writer.Close();
                    }

                    LogControl.TryResetFileName(fileInfo.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}file:{1}, content:{2}, write log exception:{3}",
                        DateTime.Now.ToString(TEXT_PREFIX_FORMAT), fileName, text, ex);
                }
            }
        }

        internal static void Write(String text)
        {
            Write(text, LogControl.GetFileFullName());
        }

        public static void Info(String text)
        {
            LogControl.TryResetFileName();

            String nowPrefix = DateTime.Now.ToString(TEXT_PREFIX_FORMAT);
            text = nowPrefix + text;
            Write(text);
        }

        public static void Info(IEnumerable<String> texts)
        {
            if (texts == null || texts.Count() == 0)
            {
                return;
            }

            LogControl.TryResetFileName();

            String inputTxt = String.Join(Environment.NewLine, texts.Select(m => DateTime.Now.ToString(TEXT_PREFIX_FORMAT) + m));
            Write(inputTxt);
        }

        protected class SimpleLogControl
        {
            public String BaseDirectory { get; protected set; }

            public DateTime Now { get; protected set; }

            public Int64 LogSize { get; set; }

            public String FileName { get; protected set; }

            public SimpleLogControl()
            {
                this.BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                this.LogSize = 20971520;//20MB
                this.Now = DateTime.Now;
                this.FileName = GetFileName("yyyyMMdd");
            }

            public void Init()
            {
                DirectoryInfo dirInfo = new DirectoryInfo(this.BaseDirectory);

                if (!dirInfo.Exists)
                {
                    dirInfo.Create();

                    return;
                }

                TryResetFileName(dirInfo);
            }

            private String GetFileName(String timeFormat)
            {
                return DateTime.Now.ToString(timeFormat) + ".log";
            }

            public String GetFileFullName()
            {
                return Path.Combine(this.BaseDirectory, this.FileName);
            }

            public Boolean TryResetFileName()
            {
                if (Now.Date == DateTime.Now.Date)
                {
                    return false;
                }

                this.Now = DateTime.Now;
                this.FileName = GetFileName("yyyyMMdd");

                return true;
            }

            public Boolean TryResetFileName(Int64 fileLength)
            {
                Boolean isCreateNewFile = TryResetFileName();
                if (isCreateNewFile)
                {
                    return true;
                }

                isCreateNewFile = fileLength >= LogSize;

                if (isCreateNewFile)
                {
                    this.FileName = GetFileName("yyyyMMdd_HHmmss");
                }

                return isCreateNewFile;
            }

            protected Boolean TryResetFileName(DirectoryInfo dirInfo)
            {
                if (dirInfo == null || !dirInfo.Exists)
                {
                    return false;
                }

                String searchPattern = DateTime.Now.ToString("yyyyMMdd") + "*.log";
                FileInfo[] logFiles = dirInfo.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

                if (logFiles != null && logFiles.Length > 1)
                {
                    this.FileName = logFiles[logFiles.Length - 1].Name;

                    return true;
                }

                return false;
            }
        }
    }
}
