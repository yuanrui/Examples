using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Simple.Common.Drawing
{
    /// <summary>
    /// Generates CAPTCHA images
    /// </summary>
    public sealed class CaptchaImage : IDisposable
    {
        private static readonly Random random = new Random();
        private string text;
        private string familyName;
        private int height;
        private Bitmap image;

        private int width;

        private bool isDirty = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaImage"/> class.
        /// </summary>
        /// <param name="text">The s.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CaptchaImage(string text, int width, int height)
        {
            this.text = text;
            SetDimensions(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaImage"/> class.
        /// </summary>
        /// <param name="text">The s.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="familyName">Name of the font family.</param>
        public CaptchaImage(string text, int width, int height, string familyName)
        {
            this.text = text;
            SetDimensions(width, height);
            SetFamilyName(familyName);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return text; }
            set { text = value; isDirty = true; }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        [Obsolete("Use the Generate() method")]
        public Image Image
        {
            get
            {
                return Generate();
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return height; }
        }

        /// <value>
        /// The dark foreground color  
        /// </value>
        public Color ForegroundDark
        {
            get
            {
                return foregroundDark;
            }
            set
            {
                foregroundDark = value;
            }
        }

        /// <value>
        /// The light foreground color 
        /// </value>
        public Color ForegroundLight
        {
            get
            {
                return foregroundLight;
            }
            set
            {
                foregroundLight = value;
            }
        }

        /// <value>
        /// The dark background color 
        /// </value>
        public Color BackgroundDark
        {
            get
            {
                return backgroundDark;
            }
            set
            {
                backgroundDark = value;
            }
        }

        /// <value>
        /// The light background color 
        /// </value>
        public Color BackgroundLight
        {
            get
            {
                return backgroundLight;
            }
            set
            {
                backgroundLight = value;
            }
        }

        /// <value>
        /// The style to be applied to the text 
        /// </value>
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                fontStyle = value;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        
        #endregion

        /// <summary>}
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="CaptchaImage"/> is reclaimed by garbage collection.
        /// </summary>}
        ~CaptchaImage()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                image.Dispose();
            }
            // free native resources
        }

        /// <summary>
        /// Sets the height and width of the image 
        /// </summary>
        /// <param name="_width">
        /// A <see cref="System.Int32"/> representing the width
        /// </param>
        /// <param name="_height">
        /// A <see cref="System.Int32"/> representing the height
        /// </param>
        public void SetDimensions(int _width, int _height)
        {
            // Check the width and height.
            if (_width <= 0)
                throw new ArgumentOutOfRangeException("_width", _width, "Argument out of range, must be greater than zero.");
            if (_height <= 0)
                throw new ArgumentOutOfRangeException("_height", _height, "Argument out of range, must be greater than zero.");
            width = _width;
            height = _height;
            isDirty = true;
        }

        /// <summary>
        /// Sets the family name of the font to be used 
        /// </summary>
        /// <param name="_familyName">
        /// A <see cref="System.String"/> representing the font name, e.g. "Arial"
        /// </param>
        public void SetFamilyName(string _familyName)
        {
            // If the named font is not installed, default to a system font.
            try
            {
                Font font = new Font(familyName, 12F);
                familyName = _familyName;
                font.Dispose();
            }
            catch (ArgumentException)
            {
                familyName = FontFamily.GenericSerif.Name;
            }
            isDirty = true;
        }

        /// <summary>
        /// Generates the Captcha image 
        /// </summary>
        /// <returns>
        /// The Captcha image as a <see cref="Image"/>.
        /// </returns>
        /// <remarks>The method will generate the image and then cache it.</remarks>
        public Image Generate()
        {
            generateImage();
            return image;
        }

        private Color backgroundLight = Color.White;
        private Color backgroundDark = Color.LightGray;
        private FontStyle fontStyle = FontStyle.Bold;
        private Color foregroundLight = Color.LightGray;
        private Color foregroundDark = Color.DarkGray;

        private void generateImage()
        {
            if (!isDirty) return;
            // Create a new 32-bit bitmap image.
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Create a graphics object for drawing.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, width, height);

                // Fill in the background.
                HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, backgroundDark, backgroundLight);
                g.FillRectangle(hatchBrush, rect);

                // Set up the text font.
                SizeF size;
                float fontSize = rect.Height + 1;
                Font font;
                // Adjust the font size until the text fits within the image.
                do
                {
                    fontSize--;
                    font = new Font(familyName, fontSize, fontStyle);
                    size = g.MeasureString(text, font);
                } while (size.Width > rect.Width);

                // Set up the text format.
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                // Create a path using the text and warp it randomly.
                GraphicsPath path = new GraphicsPath();
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, rect, format);
                Matrix matrix = new Matrix();
                matrix.Translate(0F, 0F);
                if (System.Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    float v = 4F;
                    PointF[] points =
					{
						new PointF(random.Next(rect.Width)/v, random.Next(rect.Height)/v),
						new PointF(rect.Width - random.Next(rect.Width)/v, random.Next(rect.Height)/v),
						new PointF(random.Next(rect.Width)/v, rect.Height - random.Next(rect.Height)/v),
						new PointF(rect.Width - random.Next(rect.Width)/v, rect.Height - random.Next(rect.Height)/v)
					};
                    path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
                }
                else
                {
                    float xtr = (rect.Size.Width - size.Width) / 2F + size.Width / 10F;
                    float ytr = (rect.Size.Height - size.Height) / 2F;
                    matrix.Rotate((float)random.NextDouble() * 10F - 5F);
                    matrix.Shear((float)random.NextDouble() * 1F - 0.5F, 0F);
                    matrix.Translate(xtr, ytr);
                    path.Transform(matrix);
                }
                // Draw the text.
                using (hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, foregroundLight, foregroundDark))
                {
                    g.FillPath(hatchBrush, path);

                    // Add some random noise.
                    int m = Math.Max(rect.Width, rect.Height);
                    for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
                    {
                        int x = random.Next(rect.Width);
                        int y = random.Next(rect.Height);
                        int w = random.Next(m / 50);
                        int h = random.Next(m / 50);
                        g.FillEllipse(hatchBrush, x, y, w, h);
                    }
                }
                // Clean up.
                font.Dispose();
            }

            // Set the image.
            image = bitmap;

            isDirty = false;
        }
    }
}
