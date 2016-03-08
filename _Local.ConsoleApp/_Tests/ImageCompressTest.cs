using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace _Local.ConsoleApp._Tests
{
    public static class ImageCompressTest
    {
        private static readonly string _imgUrlSetting = Path.Combine(Environment.CurrentDirectory, "url.txt");
        private static readonly string _imgSource = Path.Combine(Environment.CurrentDirectory, "Images");
        private static readonly string _imgResult = Path.Combine(Environment.CurrentDirectory, "Results");
        private static Dictionary<string, decimal> _times = new Dictionary<string, decimal>();
        private const int MaxSize = 300 * 1024;

        public static void Run()
        {
            if (!File.Exists(_imgUrlSetting))
            {
                Console.WriteLine("Urls setting({0}) not exists, please config it.", _imgUrlSetting);
                File.Create(_imgUrlSetting);
            }

            if (!Directory.Exists(_imgSource))
            {
                Directory.CreateDirectory(_imgSource);
            }

            if (!Directory.Exists(_imgResult))
            {
                Directory.CreateDirectory(_imgResult);
            }

            Console.WriteLine("Image Source:{0}{2}Compress Result:{1}", _imgSource, _imgResult, Environment.NewLine);

            //CompressFilesByDirectory(_imgSource);
            CompressFilesByUrl(_imgUrlSetting);

        }

        private static void CompressFilesByDirectory(string dirPath)
        {
            foreach (string file in Directory.GetFiles(dirPath, "*.jpg"))
            {
                GdiCompress(file);

                WpfCompress(file);

                OldGdiCompress(file);

            }
        }

        private static void CompressFilesByUrl(string txtSetting)
        {
            var urls = File.ReadAllLines(txtSetting, Encoding.UTF8);

            foreach (var url in urls)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                GdiCompress(url);

                WpfCompress(url);
            }
        }

        private static void WpfCompress(string filePath)
        {
            try
            {
                var targetFile = Path.Combine(_imgResult, Path.GetFileNameWithoutExtension(filePath) + "_wpf" + Path.GetExtension(filePath));
                var watch = Stopwatch.StartNew();
                var result = DownloadDataByUrl(filePath, MaxSize, ImageUtils.Compress);
                watch.Stop();
                File.WriteAllBytes(targetFile, result);
                Console.WriteLine("Wpf Compressed File:{0} Time:{1}s", targetFile, watch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("File:{0} Ex:{1}", filePath, ex.Message);
            }
        }

        private static void GdiCompress(string filePath)
        {
            try
            {
                var targetFile = Path.Combine(_imgResult, Path.GetFileNameWithoutExtension(filePath) + "_gdi" + Path.GetExtension(filePath));
                var watch = Stopwatch.StartNew();
                var result = DownloadDataByUrl(filePath, MaxSize, ImageUtils.CompressByGdi);
                watch.Stop();
                File.WriteAllBytes(targetFile, result);
                Console.WriteLine("Gdi Compressed File:{0} Time:{1}s", targetFile, watch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("File:{0} Ex:{1}", filePath, ex.Message);
            }
        }

        private static byte[] DownloadDataByUrl(string url, int imageMaxSize, Func<byte[], int, byte[]> compressFunc)
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
                    return compressFunc(imgData, imageMaxSize);
                }
            }
        }

        #region old version

        private static long GetImgSize(Bitmap Img)
        {
            //ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
            //EncoderParameters ep = new EncoderParameters();
            //long[] qy = new long[1];
            //qy[0] = 80;//设置压缩的比例1-100
            //EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            //ep.Param[0] = eParam;

            MemoryStream nstream = new MemoryStream();
            Img.Save(nstream, ImageFormat.Jpeg);
            //Img.Save(nstream, ici, ep);
            long rs = nstream.Length;
            nstream.Dispose();
            return rs;
        }

        private static Bitmap ComImg(Bitmap Img1, int curSize, int iSize)
        {
            long ImgLen = curSize; //GetImgSize(Img1);            
            int ImgHeight = Img1.Height;
            int ImgWidth = Img1.Width;
            Bitmap Img2 = null;// new Bitmap(Img1);
            int index = 1;
            while (ImgLen > iSize)
            {
                double ysbl = System.Math.Sqrt((double)(iSize - 2048) / ImgLen);
                if (ysbl > 0.9)
                {
                    ysbl = 0.9;
                }

                if (Img2 != null)
                {
                    Img2.Dispose();
                }

                Img2 = new Bitmap(Img1, (int)(ImgWidth * ysbl), (int)(ImgHeight * ysbl));
                ImgLen = GetImgSize(Img2);
                ImgHeight = Img2.Height;
                ImgWidth = Img2.Width;
                Console.WriteLine("run count:{0} scale:{5} length:{1} memory size:{2}/{3}/{4}", index, ImgLen, Process.GetCurrentProcess().PrivateMemorySize64, GC.GetTotalMemory(false), Environment.WorkingSet, ysbl);
                index++;
            }

            return Img2;
        }

        private static byte[] GetComImgArray(string path, int size)
        {
            FileInfo fi = new FileInfo(path);

            var img = new Bitmap(path);
            img = ComImg(img, (int)fi.Length, size);
            using (MemoryStream nstream = new MemoryStream())
            {
                img.Save(nstream, ImageFormat.Jpeg);
                return nstream.ToArray();
            }
        }

        private static void OldGdiCompress(string path)
        {
            try
            {
                var targetFile = Path.Combine(_imgResult, Path.GetFileNameWithoutExtension(path) + "_gdi_2" + Path.GetExtension(path));
                var watch = Stopwatch.StartNew();
                var result = GetComImgArray(path, MaxSize);
                watch.Stop();
                File.WriteAllBytes(targetFile, result);
                Console.WriteLine("old Gdi Compressed File:{0} Time:{1}s", targetFile, watch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("File:{0} Ex:{1}", path, ex.Message);
            }
        }

        #endregion
    }
}
