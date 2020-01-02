using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Simple.Common.Drawing
{
    public class ImageUtils
    {
        #region GDI+ Compress
        
        private static Bitmap ScaleImage(Bitmap image, double scale)
        {
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            Bitmap result = new Bitmap(image, newWidth, newHeight);
            //result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //using (Graphics g = Graphics.FromImage(result))
            //{
            //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    g.CompositingQuality = CompositingQuality.HighQuality;
            //    g.SmoothingMode = SmoothingMode.HighQuality;
            //    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //    g.DrawImage(image, 0, 0, result.Width, result.Height);
            //}
            return result;
        }

        public static byte[] CompressByGdi(byte[] byteImageIn, int targetSize, int quality = 50)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            if (byteImageIn.Length <= targetSize)
            {
                return byteImageIn;
            }

            var jpegEncoder = GetEncoderParameters(quality);
            var jpegCodec = GetJpegCodec();

            var count = 0;
            var scale = 0.99d;
            using (var ms = new MemoryStream(byteImageIn))
            {
                var bmp = (Bitmap)Image.FromStream(ms, false, true);

                while (ms.Length > targetSize)
                {
                    count++;
                    scale = GetScale(ms.Length, targetSize, count);

                    ms.SetLength(0);
                    bmp = ScaleImage(bmp, scale);

                    if (jpegCodec == null)
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);
                    }
                    else
                    {
                        bmp.Save(ms, jpegCodec, jpegEncoder);
                    }
                }
                
                if (bmp != null)
                {
                    bmp.Dispose();
                }

                return ms.ToArray();
            }
        }

        protected static EncoderParameters GetEncoderParameters(int quality = 50)
        {
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = quality;
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            return ep;
        }

        protected static ImageCodecInfo GetJpegCodec()
        {
            ImageCodecInfo[] iciList = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < iciList.Length; i++)
            {
                if (iciList[i].FormatDescription.Equals("JPEG"))
                {
                    return iciList[i];
                }
            }

            return null;
        }

        private static double GetScale(double length, double targetSize, int count)
        {
            var scale = 0.99d;
            if (count == 1)
            {
                scale = Math.Sqrt((double)targetSize / ((double)length));
                scale = Math.Round(scale, 2);

                if (scale >= 1)
                {
                    scale = 0.99d;
                }
            }
            else
            {
                scale = 0.99d;
            }

            return scale;
        }

        #endregion

        #region WPF Compress

        private static BitmapImage ResizeImage(byte[] imageData, int width, int height)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(height > 0 && width > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelHeight = height;
            newBitmap.DecodePixelWidth = width;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            RenderOptions.SetBitmapScalingMode(newBitmap, BitmapScalingMode.HighQuality);
            
            newBitmap.EndInit();
            return newBitmap;
        }

        private static BitmapImage LoadBitmapImage(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.StreamSource = new MemoryStream(bytes);
            newBitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        public static byte[] Compress(byte[] byteImageIn, int targetSize, int quality = 50)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }
            
            long length = byteImageIn.Length;
            
            if (length <= targetSize)
            {
                return byteImageIn;
            }
            byte[] result = null;
            
            var stream = new MemoryStream();
            var newBitmap = LoadBitmapImage(byteImageIn);
            var scale = 0.99d;
            var count = 0;
            try
            {
                while (length > targetSize)
                {
                    count++;
                    scale = GetScale(length, targetSize, count);

                    var width = (int)(newBitmap.PixelWidth * scale);
                    var height = (int)(newBitmap.PixelHeight * scale);

                    newBitmap = ResizeImage(byteImageIn, width, height);
                    var encoder = new JpegBitmapEncoder() { QualityLevel = quality };
                    var bitmapFrame = BitmapFrame.Create(newBitmap as BitmapSource);
                    encoder.Frames.Add(bitmapFrame);
                    encoder.Save(stream);
                    length = stream.Length;

                    if (!(length > targetSize))
                    {
                        result = stream.ToArray();
                        break;
                    }
                    
                    stream.Dispose();
                    stream = new MemoryStream();
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally 
            {
                stream.Dispose();
                if (newBitmap != null && newBitmap.StreamSource != null)
                {
                    newBitmap.StreamSource.Dispose();
                }
                stream = null;
                newBitmap = null;
            }
                        
            return result;
        }

        #endregion

        public static byte[] CompressFromUrl(string url, int targetSize, int quality = 50)
        {
            return CompressFromUrl(url, targetSize, null, quality);
        }

        public static byte[] CompressFromUrl(string url, int targetSize, Action<byte[]> beforeAction, int quality = 50)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (url.IndexOf('#') >= 0)
            {
                url = url.Replace("#", "%23");
            }

            using (WebClient webClient = new WebClient())
            {
                var imgData = webClient.DownloadData(url);

                if (beforeAction != null)
                {
                    beforeAction(imgData);
                }

                try
                {
                    if (imgData.Length < targetSize)
                    {
                        return imgData;
                    }
                    else
                    {
                        return CompressByGdi(imgData, targetSize, quality);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("使用GDI压缩异常,使用Wpf组件压缩,异常原因:{0},图片地址:{1}", ex.Message, url), "图片压缩");
                    //GDI+压缩后会修改原始imgData
                    imgData = null;
                    imgData = webClient.DownloadData(url);
                    return Compress(imgData, targetSize, quality);
                }
            }
        }

        public static Image GetImageFromPath(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }

        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                else {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }

        public static byte[] Resize(byte[] byteImageIn, double scale, int quality = 50)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            long length = byteImageIn.Length;

            byte[] result = null;

            var stream = new MemoryStream();
            var newBitmap = LoadBitmapImage(byteImageIn);
            try
            {
                var width = (int)(newBitmap.PixelWidth * scale);
                var height = (int)(newBitmap.PixelHeight * scale);

                newBitmap = ResizeImage(byteImageIn, width, height);
                var encoder = new JpegBitmapEncoder() { QualityLevel = quality };
                var bitmapFrame = BitmapFrame.Create(newBitmap as BitmapSource);
                encoder.Frames.Add(bitmapFrame);
                encoder.Save(stream);
                length = stream.Length;
                result = stream.ToArray();
                stream.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream = null;
                newBitmap = null;
            }

            return result;
        }

        public static byte[] Resize(byte[] byteImageIn, int? width, int? height, int quality = 50)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            long length = byteImageIn.Length;

            byte[] result = null;

            var stream = new MemoryStream();
            var newBitmap = LoadBitmapImage(byteImageIn);
            var oldWidth = newBitmap.PixelWidth;
            var oldHeight = newBitmap.PixelHeight;

            try
            {
                ComputeSize(ref oldWidth, ref oldHeight, width, height);

                newBitmap = ResizeImage(byteImageIn, oldWidth, oldHeight);
                var encoder = new JpegBitmapEncoder() { QualityLevel = quality };
                var bitmapFrame = BitmapFrame.Create(newBitmap as BitmapSource);
                encoder.Frames.Add(bitmapFrame);
                encoder.Save(stream);
                length = stream.Length;
                result = stream.ToArray();
                stream.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream = null;
                newBitmap = null;
            }

            return result;
        }

        private static void ComputeSize(ref int width, ref int height, int? nwidth, int? nheight)
        {
            if (nwidth.HasValue && nheight.HasValue)
            {
                width = nwidth.Value;
                height = nheight.Value;
            }
            else
            {
                if (nwidth.HasValue)
                {
                    height = (int)((double)height * nwidth.Value / width);
                    width = nwidth.Value;
                }

                if (nheight.HasValue)
                {
                    width = (int)((double)width * nheight.Value / height);
                    height = nheight.Value;
                }
            }
        }

        /// <summary>
        /// 获取特征图(取右下角图片)
        /// </summary>
        /// <param name="byteImageIn"></param>
        /// <param name="targetSize"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static byte[] GetFeatureBuffer(byte[] byteImageIn, int targetSize, int quality = 50)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            if (byteImageIn.Length <= targetSize)
            {
                return byteImageIn;
            }

            using (var srcMS = new MemoryStream(byteImageIn))
            {
                var src = (Bitmap)Image.FromStream(srcMS, false, true);

                Bitmap bmp = new Bitmap((int)((double)src.Width / 2), (int)((double)src.Height / 2));
                var destRect = new Rectangle(new Point(0, 0), bmp.Size);
                var origRect = new Rectangle(new Point((int)((double)src.Width / 2), (int)((double)src.Height / 2)), bmp.Size);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(src, destRect, origRect, GraphicsUnit.Pixel);
                }

                using (var targetMS = new MemoryStream())
                {
                    bmp.Save(targetMS, ImageFormat.Jpeg);
                    try
                    {
                        return CompressByGdi(targetMS.ToArray(), targetSize, quality);
                    }
                    catch (Exception ex)
                    {
                        bmp.Save(targetMS, ImageFormat.Jpeg);
                        return Compress(targetMS.ToArray(), targetSize, quality);
                    }
                }
            }
        }
    }
}
