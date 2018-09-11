using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace _Local.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://192.168.1.108:33119/BigFileApi";
            if (!File.Exists("url.txt"))
            {
                File.Create("url.txt");
            }
            Dictionary<string, Byte[]> cache = new Dictionary<string, byte[]>();
            var rd = new Random(Guid.NewGuid().GetHashCode());
            WebClient webClient = new WebClient();
            var input = string.Empty;
            Byte[] buffer = null;
            do
            {
                DateTime now = DateTime.Now;
                var lines = new List<string>();
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        url = "http://192.168.1.108:33119/BigFileApi";
                        var value = rd.Next(0, 1000);
                        var fileName = (value % 10) + ".jpg";
                        
                        if (! cache.ContainsKey(fileName))
                        {
                            cache.Add(fileName, File.ReadAllBytes(fileName));
                        }
                        buffer = cache[fileName];
                        var rsp = webClient.UploadData(url, "POST", buffer);

                        var rspTxt = Encoding.UTF8.GetString(rsp);
                        var fileUrl = url + "/" + rspTxt;
                        Console.WriteLine(rspTxt.Replace(Environment.NewLine, string.Empty) + " " + fileName);
                        //Console.WriteLine(fileUrl);
                        lines.Add(rspTxt);

                        url = "http://192.168.1.108:33120/BigFileApi";
                        value = rd.Next(0, 1000);
                        fileName = (value % 10) + ".jpg";
                        if (!cache.ContainsKey(fileName))
                        {
                            cache.Add(fileName, File.ReadAllBytes(fileName));
                        }
                        buffer = cache[fileName];

                        rsp = webClient.UploadData(url, "POST", buffer);
                        rspTxt = Encoding.UTF8.GetString(rsp);
                        fileUrl = url + "/" + rspTxt;
                        Console.WriteLine(rspTxt.Replace(Environment.NewLine, string.Empty) + " " + fileName);
                        //Console.WriteLine(fileUrl);
                        lines.Add(rspTxt);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                Console.WriteLine("总共耗时:{0}s", (DateTime.Now - now).Seconds);
                
                File.AppendAllLines("url.txt", lines, Encoding.UTF8);

                input = Console.ReadLine();
            } while (input != "q");
            

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
