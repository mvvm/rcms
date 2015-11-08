using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace RCms.Common.Helpers
{
    public static class WebImageHelper
    {
        private static IDictionary<string, ImageFormat> _transparencyFormats = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase)
        {
            { "png", ImageFormat.Png },
            { "gif", ImageFormat.Gif },
            { "jpeg", ImageFormat.Jpeg },
            { "jpg", ImageFormat.Jpeg },
            { "ico", ImageFormat.Icon }
        };

        public static ImageFormat ResolveImageFormat(string format)
        {
            return _transparencyFormats[format];
        }


        public static WebImage ResizeByMaxDimensions(this WebImage image, int maxWidth, int maxHeight)
        {
            ImageFormat format = null;
            if (!_transparencyFormats.TryGetValue(image.ImageFormat, out format))
            {
                throw new Exception("Unknown format");
            }

            int destW;
            int destH;
            if (image.Height <= maxHeight && image.Width <= maxWidth)
            {
                destH = image.Height;
                destW = image.Width;
            }
            else if (image.Height > image.Width)
            {
                var k = (double)maxHeight / image.Height;
                destH = maxHeight;
                destW = (int)Math.Floor(k * image.Width);
            }
            else
            {
                var k = (double)maxWidth / image.Width;
                destW = maxWidth;
                destH = (int)Math.Floor(k * image.Height);
            }

            using (Image resizedImage = new Bitmap(destW, destH))
            {
                using (Bitmap source = new Bitmap(new MemoryStream(image.GetBytes())))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, destW, destH);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, format);
                    return new WebImage(ms.ToArray());
                }
            }
        }

        public static WebImage CropAndResize(this WebImage image, int width, int height)
        {
            return image.Resize(width, height, false, false);
        }

        public static WebImage CropAndResize(this WebImage image, int top, int left, int areaWidth, int areaHeight, int destWidth, int destHeight)
        {
            ImageFormat format = null;
            if (!_transparencyFormats.TryGetValue(image.ImageFormat, out format))
            {
                throw new Exception("Unknown format");
            }

            using (Image resizedImage = new Bitmap(destWidth, destHeight))
            {
                using (Bitmap source = new Bitmap(new MemoryStream(image.GetBytes())))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        g.DrawImage(source, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(left, top, areaWidth, areaHeight), GraphicsUnit.Pixel);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, format);
                    return new WebImage(ms.ToArray());
                }
            }
        }

        public static WebImage ResizeByMaxDimensionsWithStretch(this WebImage image, int maxWidth, int maxHeight)
        {
            ImageFormat format = null;
            if (!_transparencyFormats.TryGetValue(image.ImageFormat, out format))
            {
                throw new Exception("Unknown format");
            }

            var kSource = image.Width / (double)image.Height;
            var kDest = maxWidth / (double)maxHeight;
            var destW = image.Width;
            var destH = image.Height;
            int imageH;
            int imageW;

            if (image.Width < maxWidth && image.Height < maxHeight)
            {
                var k = (maxWidth * image.Height) / ((double)maxHeight * image.Width);
                if (k < 1)
                {
                    imageH = (int)(image.Height / k);
                    imageW = image.Width;
                }
                else
                {
                    imageH = image.Height;
                    imageW = (int)(image.Width * k);
                }
            }
            else
            {
                imageW = maxWidth;
                imageH = maxHeight;

                if (kSource > kDest)
                {
                    // scale by Height
                    destH = (int)Math.Ceiling((image.Height * (double)maxWidth) / image.Width);
                    destW = maxWidth;
                }
                else
                {
                    // scale by Width
                    destW = (int)Math.Ceiling((image.Width * (double)maxHeight) / image.Height);
                    destH = maxHeight;
                }
            }


            using (Image resizedImage = new Bitmap(imageW, imageH))
            {
                using (Bitmap source = new Bitmap(new MemoryStream(image.GetBytes())))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        // Create solid brush.
                        SolidBrush blueBrush = new SolidBrush(Color.White);

                        // Create rectangle.
                        Rectangle fullRect = new Rectangle(0, 0, imageW, imageH);

                        // Fill rectangle to screen.
                        g.FillRectangle(blueBrush, fullRect);

                        // Paint image in center
                        g.DrawImage(source, (imageW - destW) / 2, (imageH - destH) / 2, destW, destH);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, format);
                    return new WebImage(ms.ToArray());
                }
            }
        }

        //public static WebImage ResizeByMaxCropOptions(this WebImage image, int destWidth, int destHeight, int x, int y, int srcWidth, int srcHeight )
        //{
        //    ImageFormat format = null;
        //    if (!_transparencyFormats.TryGetValue(image.ImageFormat, out format))
        //    {
        //        throw new Exception("Unknown format");
        //    }


        //    using (Image resizedImage = new Bitmap(destWidth, destHeight))
        //    {
        //        using (Bitmap source = new Bitmap(new MemoryStream(image.GetBytes())))
        //        {
        //            using (Graphics g = Graphics.FromImage(resizedImage))
        //            {
        //                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //                g.DrawImage(source, x, y, destW, destH);
        //            }
        //        }
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            resizedImage.Save(ms, format);
        //            return new WebImage(ms.ToArray());
        //        }
        //    }
        //}

        public static WebImage ResizePreserveTransparency(this WebImage image, int width, int height)
        {
            ImageFormat format = null;
            if (!_transparencyFormats.TryGetValue(image.ImageFormat, out format))
            {
                return image.Resize(width, height);
            }

            //keep ratio *************************************
            double ratio = (double)image.Width / image.Height;
            double desiredRatio = (double)width / height;
            if (ratio > desiredRatio)
            {
                height = Convert.ToInt32(width / ratio);
            }
            if (ratio < desiredRatio)
            {
                width = Convert.ToInt32(height * ratio);
            }
            //************************************************

            using (Image resizedImage = new Bitmap(width, height))
            {
                using (Bitmap source = new Bitmap(new MemoryStream(image.GetBytes())))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, width, height);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, format);
                    return new WebImage(ms.ToArray());
                }
            }
        }
    }
}
