using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageMap
{
    public abstract class Map : IDisposable
    {
        public const int MAP_WIDTH = 128;
        public const int MAP_HEIGHT = 128;
        public byte[] Colors { get; protected set; }
        public Bitmap Image { get; protected set; }
        public Bitmap Original { get; protected set; }

        protected Map(Bitmap original, Bitmap converted, byte[] colors)
        {
            Original = original;
            Image = converted;
            Colors = colors;
        }
        public Map(byte[] colors)
        {
            Colors = colors;
        }

        public void Dispose()
        {
            Image?.Dispose();
            Original?.Dispose();
        }

        protected static int ByteClamp(int a, int b)
        {
            int sum = a + b;
            return sum > 255 ? 255 : (sum < 0 ? 0 : sum);
        }

        protected static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            return img.Clone(cropArea, PixelFormat.DontCare);
        }

        protected static Bitmap ResizeImg(Image image, int width, int height, InterpolationMode mode, bool maintain_aspect_ratio)
        {
            int draw_height, draw_width, pos_x, pos_y;

            if (maintain_aspect_ratio)
            {
                double ratioX = (double)width / (double)image.Width;
                double ratioY = (double)height / (double)image.Height;
                double ratio = Math.Min(ratioX, ratioY);
                draw_height = (int)(image.Height * ratio);
                draw_width = (int)(image.Width * ratio);
                pos_x = (int)((width - (image.Width * ratio)) / 2);
                pos_y = (int)((height - (image.Height * ratio)) / 2);
            }
            else
            {
                draw_height = height;
                draw_width = width;
                pos_x = 0;
                pos_y = 0;
            }

            var destRect = new Rectangle(pos_x, pos_y, draw_width, draw_height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = mode;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        protected static bool IsEmpty(Bitmap image)
        {
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            image.UnlockBits(data);
            return bytes.All(x => x == 0);
        }
    }
}
