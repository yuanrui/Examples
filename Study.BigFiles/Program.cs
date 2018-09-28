using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Simple.Common.Logging;

namespace Study.BigFiles
{
    class Program
    {
        static void Main(String[] args)
        {
            String productName = GetProductName();
            Trace.Listeners.Add(new BigFileTraceListener());

            if (Environment.UserInteractive)
            {
                StartApp(productName);
            }
            else
            {
                StartWinService(productName);
            }
        }

        private static void StartApp(String productName)
        {
            Console.Title = productName;

            HttpHostManager hostManager = new HttpHostManager();
            hostManager.Start();

            EndlessLoop();

            Trace.WriteLine("Exiting...");
            Thread.Sleep(3000);
        }

        private static void StartWinService(String productName)
        {
            using (HttpWinService service = new HttpWinService())
            {
                ServiceBase.Run(service);
            }
        }

        private static String GetProductName()
        {
            FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            return fileVer.ProductName;
        }

        private static void EndlessLoop()
        {
            Trace.Write("Press 'q' to exit.");

            try
            {
                var input = string.Empty;
                do
                {
                    input = Console.ReadLine();
                } while (input != "q");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        protected class BigFileTraceListener : TraceListener
        {
            public override void Write(string message)
            {
                Console.WriteLine("{0}>>{1}", DateTime.Now.ToString("HH:mm:ss"), message);
            }

            public override void WriteLine(string message)
            {
                SimpleLogger.Info(message);
                Console.WriteLine("{0}>>{1}", DateTime.Now.ToString("HH:mm:ss"), message);
            }
        }

        #region Test Code

#if NET40

        const Int32 BIG_FILE_PORT = 33119;
        const String BIG_FILE_NAME = "BigFile.data";
        const Int64 BIG_FILE_SIZE = 10737418240;

        private static byte Checksum(byte[] data)
        {
            byte sum = 0;
            unchecked 
            {
                foreach (byte b in data)
                {
                    sum += b;
                }
            }
            return sum;
        }

        private static void TestWrite()
        {
            var bigFile = new BigFile(BIG_FILE_NAME, 10737418240);

            for (int i = 0; i < 10000000; i++)
            {
                var index = i % 10;
                var fileName = index.ToString() + ".jpg";
                var buffer = File.ReadAllBytes(fileName);
                var bufferLength = buffer.Length;
                var offset = bigFile.Write(buffer);
                var msg = string.Format("{0},{1},{2},{3}\n", offset, bufferLength, fileName, i);
                WriteMsg(BIG_FILE_NAME + ".txt", msg);

                Console.Write(msg);
            }
        }

        private static void WriteMsg(string fileName, string msg)
        {
            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var length = stream.Length;
                stream.Seek(length, SeekOrigin.Begin);
                var buffer = Encoding.UTF8.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);

                stream.Flush();
            }
        }

        private static List<String> ReadMsg(string fileName)
        {
            List<String> lines = new List<String>();
            String line;
            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);
                }
            }

            return lines;
        }
        
        private static void TestRead()
        {
            var bigFile = new BigFile(BIG_FILE_NAME, 10737418240);
            var lines = ReadMsg(BIG_FILE_NAME + ".txt");
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }
            var index = 0;
            foreach (var line in lines)
            {
                index++;
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var arry = line.Split(',');
                var fileName = string.Format("{0}-{2}-{1}", arry[0], arry[2], Guid.NewGuid().ToString("N"));
                var filePath = Path.Combine("Images", fileName);
                
                var data = bigFile.Read(Convert.ToInt64(arry[0]));

                if (data == null || data.Length == 0)
                {
                    Console.WriteLine("{0}/{1} {2} {3} 文件不存在", index, lines.Count, filePath, line);
                    continue;
                }

                Console.WriteLine("{0}/{1} {2} {3}", index, lines.Count, filePath, line);
                File.WriteAllBytes(filePath, data);
            }
        }

#endif

        #endregion
    }
}
