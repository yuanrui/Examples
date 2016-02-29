using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net;

namespace _Local.ConsoleApp
{
    class Program
    {
        private readonly static byte[] _jpgHeader = { 0xff, 0xd8 };
        private readonly static byte[] _pngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private readonly static byte[] _bmpHeader = { 0x42, 0x4D };

        const string RecordPath = "images.txt";
        const int ImageMaxSize = 300 * 1024;

        static void Main(string[] args)
        {
            try
            {
                //foreach (var item in ImageCodecInfo.GetImageDecoders())
                //{
                //    Console.WriteLine(item.CodecName);
                //}
                //foreach (var item in ImageCodecInfo.GetImageEncoders())
                //{
                //    Console.WriteLine(item.CodecName);
                //}
                var lines = File.ReadAllLines(RecordPath, Encoding.UTF8);

                foreach (var imagePath in lines)
                {
                    try
                    {
                        var data = DownloadDataByUrl(imagePath, ImageMaxSize);
                        var format = DecodeImage(data);
                        Console.WriteLine("path:{0} format:{1}", imagePath, format);
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine("path:{0} ex:{1}", imagePath, innerEx);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex);
            }

            Console.WriteLine("\nEnd...");
            Console.ReadLine();
        }

        private static byte[] DownloadDataByUrl(string url, int imageMaxSize)
        {
            using (WebClient webClient = new WebClient())
            {
                var imgData = webClient.DownloadData(url);
                if (imgData.Length < imageMaxSize)
                {
                    return imgData;
                }
                else
                {
                    return ImageUtils.Compress(imgData, imageMaxSize);
                }
            }
        }


        public static byte[] GetImageArray(string path)
        {
            using (var imgObj = Image.FromFile(path))
            {
                using (var ms = new MemoryStream())
                {
                    imgObj.Save(ms, imgObj.RawFormat);
                    return ms.ToArray();
                }
            }
        }

        public static ImageFormat DecodeImage(byte[] imageBuffer)
        {
            if (ContainsHeader(imageBuffer, _jpgHeader))
                return ImageFormat.Jpeg;

            if (ContainsHeader(imageBuffer, _pngHeader))
                return ImageFormat.Png;

            if (ContainsHeader(imageBuffer, _bmpHeader))
                return ImageFormat.Bmp;

            return ImageFormat.Bmp;
        }

        protected static bool ContainsHeader(byte[] buffer, byte[] header)
        {
            for (int i = 0; i < header.Length; i += 1)
            {
                if (header[i] != buffer[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
