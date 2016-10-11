using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Simple.Common.Utility
{
    public class ImageResizer : IDisposable
    {
        private byte[] _imageBytes;
        private readonly BitmapImage _orgBitMap;

        public ImageResizer(string path)
        {
            _imageBytes = LoadImageData(path);
            _orgBitMap = LoadBitmapImage(_imageBytes);
        }

        public ImageResizer(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
            _orgBitMap = LoadBitmapImage(_imageBytes);
        }

        public byte[] Resize(int width, ImageEncoding encoding)
        {
            return Resize(width, 0, encoding);
        }

        /// <summary>
        /// Resizes with ScaleToFill
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Resize(int width, int height, ImageEncoding encoding)
        {
            return Resize(width, height, true, encoding);
        }

        /// <summary>
        /// Resizes with ScaleToFit of crop is true
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="crop">if set to true ScaleToFill is used, if not ScaleToFit</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Resize(int width, int height, bool crop, ImageEncoding encoding)
        {
            if (width < 0)
                throw new ArgumentException("width < 0");
            if (height < 0)
                throw new ArgumentException("height < 0");

            BitmapSource bitmapSource = null;

            if (width > 0 && height > 0 && crop)
            {
                bitmapSource = ScaleToFill(width, height);
            }
            else if (width > 0 && height > 0 && !crop)
            {
                bitmapSource = ScaleToFit(width, height);
            }
            else if (width > 0)
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
            }

            _imageBytes = EncodeImageData(bitmapSource, encoding);
            return _imageBytes;
        }

        private BitmapSource ScaleToFill(int width, int height)
        {
            Contract.Requires(width > 0);
            Contract.Requires(height > 0);

            double heightRatio = height / (double)_orgBitMap.PixelHeight;
            double widthRatio = width / (double)_orgBitMap.PixelWidth;

            BitmapSource bitmapSource;
            ImageSize imageSize;

            if (heightRatio > widthRatio)
            {
                bitmapSource = ResizeImageByHeight(_imageBytes, height);
                var calc = new ImageSizeCalculator(bitmapSource.PixelWidth, height);
                imageSize = calc.ScaleToFill(width, height);
            }
            else
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
                var calc = new ImageSizeCalculator(width, bitmapSource.PixelHeight);
                imageSize = calc.ScaleToFill(width, height);
            }

            var croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect(imageSize.XOffset, imageSize.YOffset, imageSize.Width, imageSize.Height));
            return croppedBitmap;
        }

        private BitmapSource ScaleToFit(int width, int height)
        {
            Contract.Requires(width > 0);
            Contract.Requires(height > 0);

            double heightRatio = ((double)_orgBitMap.PixelHeight) / height;
            double widthRatio = ((double)_orgBitMap.PixelWidth) / width;

            if (heightRatio > widthRatio)
            {
                return ResizeImageByHeight(_imageBytes, height);
            }

            return ResizeImageByWidth(_imageBytes, width);
        }

        public void SaveToFile(string path)
        {
            SaveImageToFile(_imageBytes, path);
        }

        public void Dispose()
        {
            _imageBytes = null;
        }

        private BitmapSource ResizeImageByWidth(byte[] imageData, int width)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(width > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelWidth = width;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private BitmapSource ResizeImageByHeight(byte[] imageData, int height)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(height > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelHeight = height;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private byte[] EncodeImageData(ImageSource image, ImageEncoding encoding)
        {
            byte[] buffer = null;
            BitmapEncoder encoder = null;
            switch (encoding)
            {
                case ImageEncoding.Jpg100:
                    encoder = new JpegBitmapEncoder { QualityLevel = 100 };
                    break;

                case ImageEncoding.Jpg95:
                    encoder = new JpegBitmapEncoder { QualityLevel = 95 };
                    break;

                case ImageEncoding.Jpg90:
                    encoder = new JpegBitmapEncoder { QualityLevel = 90 };
                    break;

                case ImageEncoding.Jpg:
                    encoder = new JpegBitmapEncoder();
                    break;

                case ImageEncoding.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;

                case ImageEncoding.Png:
                    encoder = new PngBitmapEncoder();
                    break;

                case ImageEncoding.Tiff:
                    encoder = new TiffBitmapEncoder();
                    break;

                case ImageEncoding.Gif:
                    encoder = new GifBitmapEncoder();
                    break;

                case ImageEncoding.Wmp:
                    encoder = new WmpBitmapEncoder();
                    break;
            }
            if (image is BitmapSource)
            {
                var stream = new MemoryStream();
                if (encoder != null)
                {
                    var bitmapFrame = BitmapFrame.Create(image as BitmapSource);
                    encoder.Frames.Add(bitmapFrame);
                    encoder.Save(stream);
                }
                stream.Seek(0L, SeekOrigin.Begin);
                buffer = new byte[stream.Length];
                var reader = new BinaryReader(stream);
                reader.Read(buffer, 0, (int)stream.Length);
                reader.Close();
                stream.Close();
            }
            return buffer;
        }

        private static byte[] LoadImageData(string filePath)
        {
            var input = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(input);
            byte[] buffer = reader.ReadBytes((int)input.Length);
            reader.Close();
            input.Close();
            return buffer;
        }

        private BitmapImage LoadBitmapImage(byte[] bytes)
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

        private void SaveImageToFile(byte[] bytes, string path)
        {
            Contract.Requires(bytes != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(path));

            var output = new FileStream(path, FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(output);
            writer.Write(bytes);
            writer.Close();
            output.Close();
        }
    }

    public enum ImageEncoding
    {
        Jpg100, Jpg95, Jpg90, Jpg, Gif, Png, Tiff, Bmp, Wmp
    }

    public class ImageSizeCalculator
    {
        private readonly double _orgWidth;
        private readonly double _orgHeight;

        public ImageSizeCalculator(double orgWidth, double orgHeight)
        {
            _orgHeight = orgHeight;
            _orgWidth = orgWidth;
        }

        public ImageSize Scale(int newWidth)
        {
            if (newWidth <= 0)
                throw new ArgumentException(string.Format("Invalid new width: {0}", newWidth));

            if (newWidth > _orgWidth)
                throw new ArgumentException("Cannot scale up, newWidth is larger than orgWidth");

            double ratio = newWidth / _orgWidth;
            int newHeight = (int)(_orgHeight * ratio);
            return new ImageSize(newWidth, newHeight, 0, 0);
        }

        public ImageSize ScaleToFit(int newWidth, int newHeight)
        {
            if (newWidth <= 0)
                throw new ArgumentException(string.Format("Invalid new width: {0}", newWidth));
            if (newHeight <= 0)
                throw new ArgumentException(string.Format("Invalid new height: {0}", newHeight));

            double widthRatio = newWidth / _orgWidth;
            double heightRatio = newHeight / _orgHeight;

            if (heightRatio > widthRatio)
                return new ImageSize(newWidth, (int)(_orgHeight * widthRatio), 0, 0);
            return new ImageSize((int)(_orgWidth * heightRatio), newHeight, 0, 0);
        }

        public ImageSize ScaleToFill(int newWidth, int newHeight)
        {
            if (newWidth <= 0)
                throw new ArgumentException(string.Format("Invalid new width: {0}", newWidth));
            if (newHeight <= 0)
                throw new ArgumentException(string.Format("Invalid new height: {0}", newHeight));

            double widthRatio = newWidth / _orgWidth;
            double heightRatio = newHeight / _orgHeight;

            if (widthRatio > heightRatio)
                return new ImageSize(newWidth, newHeight, 0, ((int)(Math.Abs((_orgHeight * widthRatio) - newHeight)) / 2));
            return new ImageSize(newWidth, newHeight, ((int)(Math.Abs((_orgWidth * heightRatio) - newWidth)) / 2), 0);
        }
    }

    public class ImageSize
    {
        private readonly int _width;
        public int Width
        {
            get { return _width; }
        }

        private readonly int _height;
        public int Height
        {
            get { return _height; }
        }

        private readonly int _xOffset;
        public int XOffset
        {
            get { return _xOffset; }
        }

        private readonly int _yOffset;

        public ImageSize(int width, int height, int xOffset, int yOffset)
        {
            Contract.Requires(width >= 0);
            Contract.Requires(height >= 0);
            Contract.Requires(xOffset >= 0);
            Contract.Requires(yOffset >= 0);

            _width = width;
            _height = height;
            _xOffset = xOffset;
            _yOffset = yOffset;
        }

        public int YOffset
        {
            get { return _yOffset; }
        }
    }

    public class ResizeOptions
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly bool _crop;

        public ResizeOptions(int width)
        {
            _width = width;
            _name = string.Empty;
            _crop = false;
            _height = 0;
        }

        public ResizeOptions(string name, int width)
        {
            _width = width;
            _name = name;
            _crop = false;
            _height = 0;
        }

        public ResizeOptions(int width, int height, bool crop)
        {
            _width = width;
            _name = string.Empty;
            _crop = crop;
            _height = height;
        }

        /// <summary>
        /// Use to create a predefined format
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="crop"></param>
        public ResizeOptions(string name, int width, int height, bool crop)
        {
            _width = width;
            _name = name;
            _crop = crop;
            _height = height;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public bool Crop
        {
            get { return _crop; }
        }
    }

}
