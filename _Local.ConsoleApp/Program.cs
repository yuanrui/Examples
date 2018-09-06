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
            byte[] _jpgHeader = { 0xff, 0xd8 };
            byte[] _pngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            byte[] _bmpHeader = { 0x42, 0x4D };
            foreach (var item in _jpgHeader)
            {
                Console.WriteLine(item);
            }

            var url = "http://192.168.1.170:33119/BigFileApi";
            if (!File.Exists("url.txt"))
            {
                File.Create("url.txt");
            }
            var rd = new Random(Guid.NewGuid().GetHashCode());
            WebClient webClient = new WebClient();
            var input = string.Empty;
            do
            {
                var value = rd.Next(0, 1000);
                var fileName = (value % 10) + ".jpg";
                var rsp = webClient.UploadFile(url, "POST", fileName);
                var rspTxt = Encoding.UTF8.GetString(rsp);
                var fileUrl = url + "/" + rspTxt;
                Console.WriteLine(rspTxt + " " + fileName);
                Console.WriteLine(fileUrl);
                File.AppendAllText("url.txt", fileUrl, Encoding.UTF8);
                input = Console.ReadLine();
            } while (input != "q");
            

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
