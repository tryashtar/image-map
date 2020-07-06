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

    public class JavaMap : Map
    {
        public static IEnumerable<JavaMap> FromSettings(MapCreationSettings settings, IJavaVersion mapping)
        {
            Bitmap original = ResizeImg(settings.Original, MAP_WIDTH * settings.SplitW, MAP_HEIGHT * settings.SplitH, settings.InterpMode, !settings.Stretch);
            LockBitmap final = new LockBitmap((Bitmap)original.Clone());
            final.LockBits();
            // first index = which map this is
            var colors = new byte[settings.NumberOfMaps][];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new byte[MAP_WIDTH * MAP_HEIGHT];
            }

            #region java map algorithm
            for (int y = 0; y < final.Height; y++)
            {
                for (int x = 0; x < final.Width; x++)
                {
                    Color oldpixel = final.GetPixel(x, y);
                    var newpixel = GetBestPixel(oldpixel, settings.Algorithm, mapping);
                    final.SetPixel(x, y, newpixel);
                    if (settings.Dither)
                    {
                        // floyd-steinberg
                        int error_a = oldpixel.A - newpixel.A;
                        int error_r = oldpixel.R - newpixel.R;
                        int error_g = oldpixel.G - newpixel.G;
                        int error_b = oldpixel.B - newpixel.B;
                        GiveError(final, x + 1, y, error_a, error_r, error_g, error_b, 7);
                        GiveError(final, x - 1, y + 1, error_a, error_r, error_g, error_b, 3);
                        GiveError(final, x, y + 1, error_a, error_r, error_g, error_b, 5);
                        GiveError(final, x + 1, y + 1, error_a, error_r, error_g, error_b, 1);
                    }
                    int currentmap = y / MAP_HEIGHT * settings.SplitW + x / MAP_WIDTH;
                    int currentpixel = MAP_WIDTH * (y % MAP_HEIGHT) + (x % MAP_WIDTH);
                    if (newpixel == Color.FromArgb(0, 0, 0, 0))
                        colors[currentmap][currentpixel] = 0x00;
                    else
                        colors[currentmap][currentpixel] = mapping.ColorToByte(newpixel);
                }
            }
            #endregion

            final.UnlockBits();

            var maps = new List<JavaMap>();
            for (int y = 0; y < settings.SplitH; y++)
            {
                for (int x = 0; x < settings.SplitW; x++)
                {
                    Rectangle crop = new Rectangle(
                        x * original.Width / settings.SplitW,
                        y * original.Height / settings.SplitH,
                        original.Width / settings.SplitW,
                        original.Height / settings.SplitH);
                    var map_image = CropImage(final.GetImage(), crop);
                    maps.Add(new JavaMap(
                        CropImage(original, crop),
                        map_image,
                        colors[settings.SplitW * y + x]));
                }
            }
            return maps;
        }

        private static Color GetBestPixel(Color true_color, IColorAlgorithm algorithm, IJavaVersion mapping)
        {
            // partial transparency is not allowed
            if (true_color.A < 128)
                return Color.FromArgb(0, 0, 0, 0);
            if (NearestColorCache.TryGetValue(true_color, out var cached))
                return cached;
            Color best_approximate = Color.Empty;
            double mindist = Double.PositiveInfinity;
            // find the color in the palette that is closest to this one
            foreach (Color mapcolor in mapping.GetAllColors().Where(o => o.A == 255))
            {
                double distance = algorithm.Distance(true_color, mapcolor);
                if (mindist > distance)
                {
                    mindist = distance;
                    best_approximate = mapcolor;
                }
            }
            NearestColorCache.Set(true_color, best_approximate);
            return best_approximate;
        }

        protected JavaMap(Bitmap original, Bitmap converted, byte[] colors) : base(original, converted, colors)
        { }

        private static void GiveError(LockBitmap img, int x, int y, int alpha, int red, int green, int blue, int proportion)
        {
            if (x >= 0 && y >= 0 && x < img.Width && y < img.Height)
            {
                Color old = img.GetPixel(x, y);
                img.SetPixel(x, y, Color.FromArgb(
                    ByteClamp(old.A, (alpha * proportion) >> 4),
                    ByteClamp(old.R, (red * proportion) >> 4),
                    ByteClamp(old.G, (green * proportion) >> 4),
                    ByteClamp(old.B, (blue * proportion) >> 4)
                ));
            }
        }

        public JavaMap(byte[] colors, IJavaVersion mapping) : base(colors)
        {
            if (colors.Length != MAP_WIDTH * MAP_HEIGHT)
                throw new ArgumentException($"Invalid image dimensions: {colors.Length} is not {MAP_WIDTH}*{MAP_HEIGHT}");
            var canvas = new LockBitmap(MAP_WIDTH, MAP_HEIGHT);
            canvas.LockBits();
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    byte color = colors[(MAP_WIDTH * y) + x];
                    if (mapping.TryByteToColor(color, out Color col))
                        canvas.SetPixel(x, y, col);
                    else
                        canvas.SetPixel(x, y, Color.Transparent);
                }
            }
            canvas.UnlockBits();
            Image = canvas.GetImage();
            Original = Image;
        }

        // every time we learn the nearest palette entry for a color, store it here so we don't have to look it up again
        private static readonly ColorCache NearestColorCache = new ColorCache();
    }

    public class BedrockMap : Map
    {
        public static IEnumerable<BedrockMap> FromSettings(MapCreationSettings settings)
        {
            Bitmap original = ResizeImg(settings.Original, MAP_WIDTH * settings.SplitW, MAP_HEIGHT * settings.SplitH, settings.InterpMode, !settings.Stretch);
            LockBitmap final = new LockBitmap((Bitmap)original.Clone());
            final.LockBits();
            // first index = which map this is
            var colors = new byte[settings.NumberOfMaps][];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new byte[MAP_WIDTH * MAP_HEIGHT * 4];
            }

            #region bedrock map algorithm
            for (int y = 0; y < final.Height; y++)
            {
                for (int x = 0; x < final.Width; x++)
                {
                    Color realpixel = final.GetPixel(x, y);
                    Color nearest = Color.FromArgb(realpixel.A < 128 ? 0 : 255, realpixel.R, realpixel.G, realpixel.B);
                    final.SetPixel(x, y, nearest);
                    int currentmap = y / MAP_HEIGHT * settings.SplitW + x / MAP_WIDTH;
                    int byteindex = MAP_WIDTH * 4 * (y % MAP_HEIGHT) + 4 * (x % MAP_WIDTH);
                    colors[currentmap][byteindex] = nearest.R;
                    colors[currentmap][byteindex + 1] = nearest.G;
                    colors[currentmap][byteindex + 2] = nearest.B;
                    // saving the alpha works just fine, but bedrock renders each pixel fully solid or transparent
                    // it rounds (<128: invisible, >=128: solid)
                    colors[currentmap][byteindex + 3] = nearest.A;
                }
            }
            #endregion
            final.UnlockBits();

            var maps = new List<BedrockMap>();
            for (int y = 0; y < settings.SplitH; y++)
            {
                for (int x = 0; x < settings.SplitW; x++)
                {
                    Rectangle crop = new Rectangle(
                        x * original.Width / settings.SplitW,
                        y * original.Height / settings.SplitH,
                        original.Width / settings.SplitW,
                        original.Height / settings.SplitH);
                    var map_image = CropImage(final.GetImage(), crop);
                    //if (!IsEmpty(map_image))
                    maps.Add(new BedrockMap(
                        CropImage(original, crop),
                        map_image,
                        colors[settings.SplitW * y + x]));
                }
            }
            return maps;
        }
        protected BedrockMap(Bitmap original, Bitmap converted, byte[] colors) : base(original, converted, colors)
        { }

        public BedrockMap(byte[] colors) : base(colors)
        {
            if (colors.Length != MAP_WIDTH * MAP_HEIGHT * 4)
                throw new ArgumentException($"Invalid image dimensions: {colors.Length} is not {MAP_WIDTH}*{MAP_HEIGHT}*4");
            var canvas = new LockBitmap(MAP_WIDTH, MAP_HEIGHT);
            canvas.LockBits();
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    int byteindex = (MAP_WIDTH * 4 * y) + (4 * x);
                    canvas.SetPixel(x, y, Color.FromArgb(colors[byteindex + 3], colors[byteindex], colors[byteindex + 1], colors[byteindex + 2]));
                }
            }
            canvas.UnlockBits();
            Image = canvas.GetImage();
            Original = Image;
        }
    }
}
