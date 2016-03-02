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

        const string RecordPath = "images.txt";
        const int ImageMaxSize = 300 * 1024;

        static void Main(string[] args)
        {
            DownloadTest();
            //var img1Str = File.ReadAllText("img1.txt");
            //var img2Str = File.ReadAllText("img2.txt");
            ////SaveToImage(Convert.FromBase64String(img1Str), "img1.jpg");
            ////SaveToImage(Convert.FromBase64String(img2Str), "img2.jpg");
            //BitmapUtilities.Compress2(Convert.FromBase64String(img1Str), ImageMaxSize);
            //BitmapUtilities.Compress2(Convert.FromBase64String(img2Str), ImageMaxSize);
            ////File.WriteAllBytes("aaa.jpg", new ImageResizer(Convert.FromBase64String(img1Str)).Resize(500, 500, ImageEncoding.Jpg100));
            Console.WriteLine("\nEnd...");
            Console.ReadLine();
        }

        private static void SaveToImage(byte[] imgArray, string filePath)
        {
            using (var ms = new MemoryStream(imgArray))
            {
                Image.FromStream(ms).Save(filePath);
            }
        }

        private static void DownloadTest()
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
                    return BitmapUtilities.Compress2(imgData, imageMaxSize);
                    //return ImageUtils.Compress(imgData, imageMaxSize);
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

        public static BitmapFrame ReadBitmapFrame(MemoryStream photoStream)
        {
            var photoDecoder = BitmapDecoder.Create(
                photoStream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.None);
            return photoDecoder.Frames[0];
        }

        public static BitmapFrame FastResize(BitmapFrame photo, int width, int height)
        {
            var target = new TransformedBitmap(
                photo,
                new ScaleTransform(
                    width / photo.Width * 96 / photo.DpiX,
                    height / photo.Height * 96 / photo.DpiY,
                    0, 0));
            return BitmapFrame.Create(target);
        }

        public static byte[] ToByteArray(BitmapFrame targetFrame, int quality)
        {
            byte[] targetBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                var targetEncoder = new JpegBitmapEncoder
                {
                    QualityLevel = quality
                };
                targetEncoder.Frames.Add(targetFrame);
                targetEncoder.Save(memoryStream);
                targetBytes = memoryStream.ToArray();
            }
            return targetBytes;
        }
    }

    public static class BitmapUtilities
    {
        public static BitmapFrame Draw(this BitmapFrame photo, params Drawing[] drawings)
        {
            return Draw(photo, (int)photo.Width, (int)photo.Height, drawings);
        }

        public static BitmapFrame Draw(this BitmapFrame photo, int width, int height, params Drawing[] drawings)
        {
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            foreach (var drawing in drawings)
            {
                group.Children.Add(drawing);
            }
            var targetVisual = new DrawingVisual();
            var targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(group);
            targetContext.Close();
            var target = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Default);
            target.Render(targetVisual);
            var targetFrame = BitmapFrame.Create(target);
            return targetFrame;
        }

        public static BitmapFrame ReadBitmapFrame(MemoryStream photoStream)
        {
            var photoDecoder = BitmapDecoder.Create(
                photoStream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.None);
            return photoDecoder.Frames[0];
        }

        public static BitmapFrame Resize(this BitmapFrame photo, double width, double height, double offsetX, double offsetY)
        {
            var target = new TransformedBitmap(
                photo,
                new ScaleTransform(
                    width / photo.Width * 96 / photo.DpiX,
                    height / photo.Height * 96 / photo.DpiY,
                    offsetX, offsetY));
            return BitmapFrame.Create(target);
        }

        public static BitmapFrame Resize(this BitmapFrame photo, double width, double height)
        {
            return Resize(photo, width, height, 0, 0);
        }

        public static BitmapFrame FastResize(BitmapFrame photo, int width, int height)
        {
            var target = new TransformedBitmap(
                photo,
                new ScaleTransform(
                    width / photo.Width * 96 / photo.DpiX,
                    height / photo.Height * 96 / photo.DpiY,
                    0, 0));
            return BitmapFrame.Create(target);
        }

        public static byte[] ToByteArray(this BitmapFrame targetFrame)
        {
            byte[] targetBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                var targetEncoder = new PngBitmapEncoder();
                targetEncoder.Frames.Add(targetFrame);
                targetEncoder.Save(memoryStream);
                targetBytes = memoryStream.ToArray();
            }
            return targetBytes;
        }

        public static byte[] Compress(byte[] byteImageIn, int targetSize)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            if (byteImageIn.Length <= targetSize)
            {
                return byteImageIn;
            }

            using (var ms = new MemoryStream(byteImageIn))
            {
                var bitmapFrame = ReadBitmapFrame(ms);
                while (ms.Length > targetSize)
                {
                    var scale = Math.Sqrt((double)targetSize / (double)ms.Length);
                    ms.SetLength(0);
                    bitmapFrame = bitmapFrame.Resize(scale * bitmapFrame.Width, scale * bitmapFrame.Height);
                    
                    var targetEncoder = new JpegBitmapEncoder();
                    targetEncoder.QualityLevel = 100;
                    targetEncoder.Frames.Add(bitmapFrame);
                    targetEncoder.Save(ms);
                }
                var reault = ms.ToArray();
                File.WriteAllBytes(Guid.NewGuid() + ".jpg", reault);
                return reault;
            }
        }

        private static BitmapImage ResizeImage(byte[] imageData, int height, int width)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(height > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelHeight = height;
            newBitmap.DecodePixelWidth = width;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private static BitmapImage LoadBitmapImage(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.StreamSource = new MemoryStream(bytes);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        public static byte[] Compress2(byte[] byteImageIn, int targetSize)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            if (byteImageIn.Length <= targetSize)
            {
                return byteImageIn;
            }
            long length = byteImageIn.Length;
            byte[] reault = null;
            var stream = new MemoryStream();
            BitmapImage newBitmap = LoadBitmapImage(byteImageIn);
            var index = 0;
            Console.WriteLine("current length:{0} ", length);
            while (length > targetSize)
            {   
                var scale = Math.Sqrt((double)targetSize / ((double)length));
                scale = Math.Round(scale, 4, MidpointRounding.AwayFromZero);
                if (scale > 0.9)
                {
                    scale = 0.9;
                }
                var width = (int)(newBitmap.PixelWidth * scale);
                var height = (int)(newBitmap.PixelHeight * scale);
                newBitmap = ResizeImage(byteImageIn, height, width);
                
                var encoder = new JpegBitmapEncoder() { QualityLevel = 100 };
                var bitmapFrame = BitmapFrame.Create(newBitmap as BitmapSource);
                encoder.Frames.Add(bitmapFrame);
                encoder.Save(stream);
                length = stream.Length;

                index++;
                Console.WriteLine("length:{5} scale:{6} run count:{0} old width:{1} height:{2}, new width:{3} height:{4}.", index, newBitmap.PixelWidth, newBitmap.PixelHeight, width, height, length, scale);

                if (! (length > targetSize))
                {
                    reault = stream.ToArray();
                    break;
                }
                
                stream.Dispose();
                stream = new MemoryStream();
            }
            stream.Dispose();
            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "cache." + Guid.NewGuid() + ".jpg"), reault);
            return reault;
        }
    }
}
