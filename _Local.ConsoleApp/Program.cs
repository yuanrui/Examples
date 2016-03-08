using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Diagnostics.Contracts;

namespace _Local.ConsoleApp
{
    class Program
    {
        private readonly static byte[] _jpgHeader = { 0xff, 0xd8 };
        private readonly static byte[] _pngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private readonly static byte[] _bmpHeader = { 0x42, 0x4D };
        
        static void Main(string[] args)
        {
            _Local.ConsoleApp._Tests.ImageCompressTest.Run();

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
