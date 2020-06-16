using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace ImageMap
{
    // stolen from https://github.com/Zaid-Ajaj/Image-Processor/blob/master/ImageFilter/LockBitmap.cs (apparently)
    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        public LockBitmap(int width, int height)
        {
            this.source = new Bitmap(width, height);
        }

        public Bitmap GetImage()
        {
            return source;
        }

        public void LockBits()
        {
            Width = source.Width;
            Height = source.Height;
            int PixelCount = Width * Height;
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            Depth = Bitmap.GetPixelFormatSize(source.PixelFormat);
            if (Depth != 8 && Depth != 24 && Depth != 32)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }
            bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat);
            int step = Depth / 8;
            Pixels = new byte[PixelCount * step];
            Iptr = bitmapData.Scan0;
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
        }

        public void UnlockBits()
        {
            Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
            source.UnlockBits(bitmapData);
        }

        public Color GetPixel(int x, int y)
        {
            int cCount = Depth / 8;
            int i = ((y * Width) + x) * cCount;
            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                return Color.FromArgb(a, r, g, b);
            }
            else if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                return Color.FromArgb(r, g, b);
            }
            else if (Depth == 8) // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                return Color.FromArgb(c, c, c);
            }
            return Color.Empty;
        }

        public void SetPixel(int x, int y, Color color)
        {
            int cCount = Depth / 8;
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            else if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            else if (Depth == 8) // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
    }
}
