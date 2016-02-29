using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Media.Imaging;

namespace _Local.ConsoleApp
{
    public class ImageUtils
    {
        private static Bitmap ScaleImage(Bitmap image, double scale)
        {
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            Bitmap result = new Bitmap(image, newWidth, newHeight);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }
            return result;
        }

        private static void SaveTemporary(Bitmap bmp, MemoryStream ms, int quality)
        {
            var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var codec = GetImageCodecInfo(bmp.RawFormat);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            bmp.Save(ms, codec, encoderParams);
        }

        private static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return codecs[0];
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
                
                var bmp = (Bitmap)Image.FromStream(ms, false, true);

                while (ms.Length > targetSize)
                {
                    var scale = Math.Sqrt((double)targetSize / (double)ms.Length);
                    ms.SetLength(0);
                    bmp = ScaleImage(bmp, scale);
                    //bmp.Save(ms, ImageCodecInfo.GetImageEncoders()[1], new EncoderParameters());
                    SaveTemporary(bmp, ms, 100);
                }
                //bmp.Save("D:\\Test.jpg", ImageFormat.Jpeg);
                if (bmp != null)
                {
                    bmp.Dispose();
                }

                return ms.ToArray();
            }
        }

    }
}
