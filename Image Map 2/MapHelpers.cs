using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Image_Map
{
    public abstract class Map
    {
        public const int MAP_WIDTH = 128;
        public const int MAP_HEIGHT = 128;
        public byte[] Colors { get; protected set; }
        public LockBitmap Image { get; protected set; }
        public Bitmap Original { get; protected set; }

        protected Map(Bitmap original, Bitmap converted, byte[] colors)
        {
            Original = original;
            Image = new LockBitmap((Bitmap)converted.Clone());
            Colors = colors;
        }
        public Map(byte[] colors)
        {
            Colors = colors;
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

        protected static Bitmap ResizeImg(Image image, int width, int height, InterpolationMode mode)
        {
            var destRect = new Rectangle(0, 0, width, height);
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
    }

    public class MapCreationSettings
    {
        public Bitmap Original { get; private set; }
        public int SplitW { get; private set; }
        public int SplitH { get; private set; }
        public InterpolationMode InterpMode { get; private set; }
        public bool Dither { get; private set; }

        public MapCreationSettings(Bitmap original, int splitW, int splitH, InterpolationMode interpMode, bool dither)
        {
            Original = original;
            SplitW = splitW;
            SplitH = splitH;
            InterpMode = interpMode;
            Dither = dither;
        }
    }

    public class JavaMap : Map
    {
        public static IEnumerable<JavaMap> FromSettings(MapCreationSettings settings)
        {
            Bitmap original = ResizeImg(settings.Original, MAP_WIDTH * settings.SplitW, MAP_HEIGHT * settings.SplitH, settings.InterpMode);
            LockBitmap final = new LockBitmap((Bitmap)original.Clone());
            final.LockBits();
            // first index = which map this is
            var colors = new byte[settings.SplitW * settings.SplitH][];
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
                    Color newpixel = Color.Empty;
                    // partial transparency is not allowed
                    if (oldpixel.A < 128)
                        newpixel = Color.FromArgb(0, 0, 0, 0);
                    else
                    {
                        if (!NearestColorCache.TryGetValue(oldpixel, out newpixel))
                        {
                            double mindist = Double.PositiveInfinity;
                            // find the color in the palette that is closest to this one
                            foreach (Color mapcolor in ColorToByte.Keys.Where(o => o.A == 255))
                            {
                                double distance = ColorDistance(oldpixel, mapcolor);
                                if (mindist > distance)
                                {
                                    mindist = distance;
                                    newpixel = mapcolor;
                                }
                            }
                            NearestColorCache[oldpixel] = newpixel;
                        }
                    }
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
                        colors[currentmap][currentpixel] = ColorToByte[newpixel];
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
                    maps.Add(new JavaMap(
                        CropImage(original, crop),
                        CropImage(final.GetImage(), crop),
                        colors[settings.SplitW * y + x]));
                }
            }
            return maps;
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

        public JavaMap(byte[] colors) : base(colors)
        {
            if (colors.Length != MAP_WIDTH * MAP_HEIGHT)
                throw new ArgumentException($"Invalid image dimensions: {colors.Length} is not {MAP_WIDTH}*{MAP_HEIGHT}");
            Image = new LockBitmap(MAP_WIDTH, MAP_HEIGHT);
            Image.LockBits();
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    byte color = colors[(MAP_WIDTH * y) + x];
                    if (ByteToColor.TryGetValue(color, out Color col))
                        Image.SetPixel(x, y, col);
                    else
                        Image.SetPixel(x, y, Color.Transparent);
                }
            }
            Image.UnlockBits();
            Original = Image.GetImage();
        }

        #region map conversion helpers
        private static Dictionary<Color, byte> ColorToByte;
        private static Dictionary<byte, Color> ByteToColor;
        // every time we learn the nearest palette entry for a color, store it here so we don't have to look it up again
        private static Dictionary<Color, Color> NearestColorCache;
        static JavaMap()
        {
            // java's color map: stores the bytes and the RGB color they correspond to on a map
            ColorToByte = new Dictionary<Color, byte>()
            {
                #region color definitions
                { Color.FromArgb(0,0,0,0), 0x00 },
                { Color.FromArgb(88,124,39), 0x04 },
                { Color.FromArgb(108,151,47), 0x05 },
                { Color.FromArgb(126,176,55), 0x06 },
                { Color.FromArgb(66,93,29), 0x07 },
                { Color.FromArgb(172,162,114), 0x08 },
                { Color.FromArgb(210,199,138), 0x09 },
                { Color.FromArgb(244,230,161), 0x0a },
                { Color.FromArgb(128,122,85), 0x0b },
                { Color.FromArgb(138,138,138), 0x0c },
                { Color.FromArgb(169,169,169), 0x0d },
                { Color.FromArgb(197,197,197), 0x0e },
                { Color.FromArgb(104,104,104), 0x0f },
                { Color.FromArgb(178,0,0), 0x10 },
                { Color.FromArgb(217,0,0), 0x11 },
                { Color.FromArgb(252,0,0), 0x12 },
                { Color.FromArgb(133,0,0), 0x13 },
                { Color.FromArgb(111,111,178), 0x14 },
                { Color.FromArgb(136,136,217), 0x15 },
                { Color.FromArgb(158,158,252), 0x16 },
                { Color.FromArgb(83,83,133), 0x17 },
                { Color.FromArgb(116,116,116), 0x18 },
                { Color.FromArgb(142,142,142), 0x19 },
                { Color.FromArgb(165,165,165), 0x1a },
                { Color.FromArgb(87,87,87), 0x1b },
                { Color.FromArgb(0,86,0), 0x1c },
                { Color.FromArgb(0,105,0), 0x1d },
                { Color.FromArgb(0,123,0), 0x1e },
                { Color.FromArgb(0,64,0), 0x1f },
                { Color.FromArgb(178,178,178), 0x20 },
                { Color.FromArgb(217,217,217), 0x21 },
                { Color.FromArgb(252,252,252), 0x22 },
                { Color.FromArgb(133,133,133), 0x23 },
                { Color.FromArgb(114,117,127), 0x24 },
                { Color.FromArgb(139,142,156), 0x25 },
                { Color.FromArgb(162,166,182), 0x26 },
                { Color.FromArgb(85,87,96), 0x27 },
                { Color.FromArgb(105,75,53), 0x28 },
                { Color.FromArgb(128,93,65), 0x29 },
                { Color.FromArgb(149,108,76), 0x2a },
                { Color.FromArgb(78,56,40), 0x2b },
                { Color.FromArgb(78,78,78), 0x2c },
                { Color.FromArgb(95,95,95), 0x2d },
                { Color.FromArgb(111,111,111), 0x2e },
                { Color.FromArgb(58,58,58), 0x2f },
                { Color.FromArgb(44,44,178), 0x30 },
                { Color.FromArgb(54,54,217), 0x31 },
                { Color.FromArgb(63,63,252), 0x32 },
                { Color.FromArgb(33,33,133), 0x33 },
                { Color.FromArgb(99,83,49), 0x34 },
                { Color.FromArgb(122,101,61), 0x35 },
                { Color.FromArgb(141,118,71), 0x36 },
                { Color.FromArgb(74,62,38), 0x37 },
                { Color.FromArgb(178,175,170), 0x38 },
                { Color.FromArgb(217,214,209), 0x39 },
                { Color.FromArgb(252,249,242), 0x3a },
                { Color.FromArgb(133,131,127), 0x3b },
                { Color.FromArgb(150,88,36), 0x3c },
                { Color.FromArgb(184,108,43), 0x3d },
                { Color.FromArgb(213,126,50), 0x3e },
                { Color.FromArgb(113,66,27), 0x3f },
                { Color.FromArgb(124,52,150), 0x40 },
                { Color.FromArgb(151,64,184), 0x41 },
                { Color.FromArgb(176,75,213), 0x42 },
                { Color.FromArgb(93,40,113), 0x43 },
                { Color.FromArgb(71,107,150), 0x44 },
                { Color.FromArgb(87,130,184), 0x45 },
                { Color.FromArgb(101,151,213), 0x46 },
                { Color.FromArgb(53,80,113), 0x47 },
                { Color.FromArgb(159,159,36), 0x48 },
                { Color.FromArgb(195,195,43), 0x49 },
                { Color.FromArgb(226,226,50), 0x4a },
                { Color.FromArgb(120,120,27), 0x4b },
                { Color.FromArgb(88,142,17), 0x4c },
                { Color.FromArgb(108,174,21), 0x4d },
                { Color.FromArgb(126,202,25), 0x4e },
                { Color.FromArgb(66,107,13), 0x4f },
                { Color.FromArgb(168,88,115), 0x50 },
                { Color.FromArgb(206,108,140), 0x51 },
                { Color.FromArgb(239,126,163), 0x52 },
                { Color.FromArgb(126,66,86), 0x53 },
                { Color.FromArgb(52,52,52), 0x54 },
                { Color.FromArgb(64,64,64), 0x55 },
                { Color.FromArgb(75,75,75), 0x56 },
                { Color.FromArgb(40,40,40), 0x57 },
                { Color.FromArgb(107,107,107), 0x58 },
                { Color.FromArgb(130,130,130), 0x59 },
                { Color.FromArgb(151,151,151), 0x5a },
                { Color.FromArgb(80,80,80), 0x5b },
                { Color.FromArgb(52,88,107), 0x5c },
                { Color.FromArgb(64,108,130), 0x5d },
                { Color.FromArgb(75,126,151), 0x5e },
                { Color.FromArgb(40,66,80), 0x5f },
                { Color.FromArgb(88,43,124), 0x60 },
                { Color.FromArgb(108,53,151), 0x61 },
                { Color.FromArgb(126,62,176), 0x62 },
                { Color.FromArgb(66,33,93), 0x63 },
                { Color.FromArgb(36,52,124), 0x64 },
                { Color.FromArgb(43,64,151), 0x65 },
                { Color.FromArgb(50,75,176), 0x66 },
                { Color.FromArgb(27,40,93), 0x67 },
                { Color.FromArgb(71,52,36), 0x68 },
                { Color.FromArgb(87,64,43), 0x69 },
                { Color.FromArgb(101,75,50), 0x6a },
                { Color.FromArgb(53,40,27), 0x6b },
                { Color.FromArgb(71,88,36), 0x6c },
                { Color.FromArgb(87,108,43), 0x6d },
                { Color.FromArgb(101,126,50), 0x6e },
                { Color.FromArgb(53,66,27), 0x6f },
                { Color.FromArgb(107,36,36), 0x70 },
                { Color.FromArgb(130,43,43), 0x71 },
                { Color.FromArgb(151,50,50), 0x72 },
                { Color.FromArgb(80,27,27), 0x73 },
                { Color.FromArgb(17,17,17), 0x74 },
                { Color.FromArgb(21,21,21), 0x75 },
                { Color.FromArgb(25,25,25), 0x76 },
                { Color.FromArgb(13,13,13), 0x77 },
                { Color.FromArgb(174,166,53), 0x78 },
                { Color.FromArgb(212,203,65), 0x79 },
                { Color.FromArgb(247,235,76), 0x7a },
                { Color.FromArgb(130,125,40), 0x7b },
                { Color.FromArgb(63,152,148), 0x7c },
                { Color.FromArgb(78,186,181), 0x7d },
                { Color.FromArgb(91,216,210), 0x7e },
                { Color.FromArgb(47,114,111), 0x7f },
                { Color.FromArgb(51,89,178), 0x80 },
                { Color.FromArgb(62,109,217), 0x81 },
                { Color.FromArgb(73,126,252), 0x82 },
                { Color.FromArgb(39,66,133), 0x83 },
                { Color.FromArgb(0,151,40), 0x84 },
                { Color.FromArgb(0,185,49), 0x85 },
                { Color.FromArgb(0,214,57), 0x86 },
                { Color.FromArgb(0,113,30), 0x87 },
                { Color.FromArgb(90,59,34), 0x88 },
                { Color.FromArgb(110,73,42), 0x89 },
                { Color.FromArgb(127,85,48), 0x8a },
                { Color.FromArgb(67,44,25), 0x8b },
                { Color.FromArgb(78,1,0), 0x8c },
                { Color.FromArgb(95,1,0), 0x8d },
                { Color.FromArgb(111,2,0), 0x8e },
                { Color.FromArgb(58,1,0), 0x8f },
                { Color.FromArgb(145,123,112), 0x90 },
                { Color.FromArgb(178,150,136), 0x91 },
                { Color.FromArgb(207,175,159), 0x92 },
                { Color.FromArgb(109,92,84), 0x93 },
                { Color.FromArgb(111,56,25), 0x94 },
                { Color.FromArgb(135,69,31), 0x95 },
                { Color.FromArgb(157,81,36), 0x96 },
                { Color.FromArgb(83,42,19), 0x97 },
                { Color.FromArgb(104,60,75), 0x98 },
                { Color.FromArgb(126,74,92), 0x99 },
                { Color.FromArgb(147,86,107), 0x9a },
                { Color.FromArgb(77,45,56), 0x9b },
                { Color.FromArgb(78,75,96), 0x9c },
                { Color.FromArgb(95,92,118), 0x9d },
                { Color.FromArgb(111,107,136), 0x9e },
                { Color.FromArgb(58,56,72), 0x9f },
                { Color.FromArgb(129,92,25), 0xa0 },
                { Color.FromArgb(158,113,31), 0xa1 },
                { Color.FromArgb(184,131,36), 0xa2 },
                { Color.FromArgb(97,69,19), 0xa3 },
                { Color.FromArgb(71,81,37), 0xa4 },
                { Color.FromArgb(87,99,44), 0xa5 },
                { Color.FromArgb(102,116,52), 0xa6 },
                { Color.FromArgb(53,60,28), 0xa7 },
                { Color.FromArgb(111,53,54), 0xa8 },
                { Color.FromArgb(136,65,66), 0xa9 },
                { Color.FromArgb(158,76,77), 0xaa },
                { Color.FromArgb(83,40,41), 0xab },
                { Color.FromArgb(40,28,24), 0xac },
                { Color.FromArgb(48,35,30), 0xad },
                { Color.FromArgb(56,41,35), 0xae },
                { Color.FromArgb(30,21,18), 0xaf },
                { Color.FromArgb(94,74,68), 0xb0 },
                { Color.FromArgb(115,91,83), 0xb1 },
                { Color.FromArgb(133,106,97), 0xb2 },
                { Color.FromArgb(70,55,50), 0xb3 },
                { Color.FromArgb(60,63,63), 0xb4 },
                { Color.FromArgb(74,78,78), 0xb5 },
                { Color.FromArgb(86,91,91), 0xb6 },
                { Color.FromArgb(45,47,47), 0xb7 },
                { Color.FromArgb(85,50,61), 0xb8 },
                { Color.FromArgb(104,61,74), 0xb9 },
                { Color.FromArgb(121,72,87), 0xba },
                { Color.FromArgb(63,38,45), 0xbb },
                { Color.FromArgb(52,42,63), 0xbc },
                { Color.FromArgb(64,52,78), 0xbd },
                { Color.FromArgb(75,61,91), 0xbe },
                { Color.FromArgb(40,32,47), 0xbf },
                { Color.FromArgb(52,35,24), 0xc0 },
                { Color.FromArgb(64,42,30), 0xc1 },
                { Color.FromArgb(75,49,35), 0xc2 },
                { Color.FromArgb(40,26,18), 0xc3 },
                { Color.FromArgb(52,56,29), 0xc4 },
                { Color.FromArgb(64,69,36), 0xc5 },
                { Color.FromArgb(75,81,42), 0xc6 },
                { Color.FromArgb(40,42,22), 0xc7 },
                { Color.FromArgb(99,42,32), 0xc8 },
                { Color.FromArgb(121,50,39), 0xc9 },
                { Color.FromArgb(140,59,45), 0xca },
                { Color.FromArgb(74,31,24), 0xcb },
                { Color.FromArgb(26,15,11), 0xcc },
                { Color.FromArgb(31,18,13), 0xcd },
                { Color.FromArgb(37,22,16), 0xce },
                { Color.FromArgb(19,11,8), 0xcf },
                #endregion
            };
            ByteToColor = ColorToByte.ToDictionary(x => x.Value, x => x.Key);
            NearestColorCache = new Dictionary<Color, Color>();
        }

        // color distance algorithm I stole from https://stackoverflow.com/a/33782458
        // seems to work legitimately better and quicker than more sophisticated algorithms
        private static double ColorDistance(Color e1, Color e2)
        {
            long rmean = ((long)e1.R + e2.R) / 2;
            long r = (long)e1.R - e2.R;
            long g = (long)e1.G - e2.G;
            long b = (long)e1.B - e2.B;
            return (((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8);
        }
        #endregion
    }

    public class BedrockMap : Map
    {
        public static IEnumerable<BedrockMap> FromSettings(MapCreationSettings settings)
        {
            Bitmap original = ResizeImg(settings.Original, MAP_WIDTH * settings.SplitW, MAP_HEIGHT * settings.SplitH, settings.InterpMode);
            LockBitmap final = new LockBitmap((Bitmap)original.Clone());
            final.LockBits();
            // first index = which map this is
            var colors = new byte[settings.SplitW * settings.SplitH][];
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
                    maps.Add(new BedrockMap(
                        CropImage(original, crop),
                        CropImage(final.GetImage(), crop),
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
            Image = new LockBitmap(MAP_WIDTH, MAP_HEIGHT);
            Image.LockBits();
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    int byteindex = (MAP_WIDTH * 4 * y) + (4 * x);
                    Image.SetPixel(x, y, Color.FromArgb(colors[byteindex + 3], colors[byteindex], colors[byteindex + 1], colors[byteindex + 2]));
                }
            }
            Image.UnlockBits();
            Original = Image.GetImage();
        }
    }

    public abstract class MinecraftWorld
    {
        protected Dictionary<long, Map> Maps;
        protected const string LOCAL_IDENTIFIER = "~local";
        public IReadOnlyDictionary<long, Map> WorldMaps => Maps;
        public string Folder { get; protected set; }
        public string Name { get; protected set; }
        public MinecraftWorld(string folder)
        {
            Folder = folder;
        }
        // user needs to call this
        public void Initialize()
        {
            Maps = LoadMaps();
        }
        public abstract void AddMaps(Dictionary<long, Map> maps);
        public abstract void RemoveMap(long mapid);
        public bool AddChestsLocalPlayer(IEnumerable<long> mapids)
        {
            return AddChests(mapids, LOCAL_IDENTIFIER);
        }

        // returns whether there was enough room to fit the chests
        public abstract bool AddChests(IEnumerable<long> mapids, string playerid);
        public abstract IEnumerable<string> GetPlayerIDs();
        protected abstract Dictionary<long, Map> LoadMaps();
        // returns slot IDs not occupied with an item
        protected abstract IEnumerable<byte> GetFreeSlots(NbtList invtag);
        // mapids count must not exceed 27
        protected abstract NbtCompound CreateChest(IEnumerable<long> mapids);
        // returns whether there was enough room to fit the chests
        protected bool PutChestsInInventory(NbtList invtag, IEnumerable<long> mapids)
        {
            // add to chests one by one
            var slots = GetFreeSlots(invtag);
            int total = mapids.Count();
            int current = 0;
            foreach (var slot in slots)
            {
                var chestcontents = mapids.Skip(current).Take(27);
                var chest = CreateChest(chestcontents);
                chest.Add(new NbtByte("Slot", slot));
                // bedrock-specific lines, replace existing item in this slot which should only be air
                var existingitem = invtag.Where(x => x["Slot"].ByteValue == slot).FirstOrDefault();
                if (existingitem != null)
                {
                    invtag.Insert(invtag.IndexOf(existingitem), chest);
                    invtag.Remove(existingitem);
                }
                else
                {
                    invtag.ListType = NbtTagType.Compound;
                    invtag.Add(chest);
                }
                current += 27;
                if (current >= total)
                    return true;
            }
            return false;
        }
        protected static bool MapString(string input, out long mapid)
        {
            bool success = Regex.Match(input, @"^map_-?\d+$").Success;
            if (success)
                mapid = Int64.Parse(Regex.Match(input, @"-?\d+").Value);
            else
                mapid = 0;
            return success;
        }
    }

    public class JavaWorld : MinecraftWorld
    {
        private NbtFile LevelDat;
        private readonly bool HasLocalPlayer;

        public JavaWorld(string folder) : base(folder)
        {
            LevelDat = new NbtFile(Path.Combine(folder, "level.dat"));
            Name = LevelDat.RootTag["Data"]["LevelName"].StringValue;
            HasLocalPlayer = (LevelDat.RootTag["Data"]["Player"] != null);
        }

        public override void AddMaps(Dictionary<long, Map> maps)
        {
            foreach (var map in maps)
            {
                NbtCompound mapfile = new NbtCompound("map")
                {
                    new NbtCompound("data")
                    {
                        new NbtByte("scale", 0),
                        new NbtByte("dimension", 0),
                        new NbtShort("height", Map.MAP_HEIGHT),
                        new NbtShort("width", Map.MAP_WIDTH),
                        new NbtByte("trackingPosition", 0),
                        new NbtByte("unlimitedTracking", 0),
                        new NbtInt("xCenter", Int32.MaxValue),
                        new NbtInt("zCenter", Int32.MaxValue),
                        new NbtByte("locked", 1),
                        new NbtByteArray("colors", map.Value.Colors)
                    }
                };
                new NbtFile(mapfile).SaveToFile(MapFileLocation(map.Key), NbtCompression.GZip);
            }
        }

        public override void RemoveMap(long mapid)
        {
            File.Delete(MapFileLocation(mapid));
        }

        public override IEnumerable<string> GetPlayerIDs()
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(Folder, "playerdata"), "*.dat"))
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }

        public override bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            // acquire the file this player is stored in, and the tag that represents said player
            NbtCompound playertag;
            NbtFile activefile;
            if (playerid == LOCAL_IDENTIFIER)
            {
                if (!HasLocalPlayer)
                    throw new InvalidOperationException("Requested local player but there is none for this world");
                activefile = LevelDat;
                playertag = (NbtCompound)activefile.RootTag["Data"]["Player"];
            }
            else
            {
                activefile = new NbtFile(PlayerFileLocation(playerid));
                playertag = activefile.RootTag;
            }
            var invtag = (NbtList)playertag["Inventory"];

            var success = PutChestsInInventory(invtag, mapids);

            activefile.SaveToFile(activefile.FileName, NbtCompression.GZip);
            return success;
        }

        protected override Dictionary<long, Map> LoadMaps()
        {
            var maps = new Dictionary<long, Map>();
            foreach (string file in Directory.GetFiles(Path.Combine(Folder, "data"), "*.dat"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (MapString(name, out long number))
                {
                    NbtFile nbtfile = new NbtFile(file);
                    maps.Add(number, new JavaMap(nbtfile.RootTag["data"]["colors"].ByteArrayValue));
                }
            }
            return maps;
        }

        protected override IEnumerable<byte> GetFreeSlots(NbtList invtag)
        {
            List<byte> emptyslots = new List<byte>(35);
            for (byte i = 0; i < 35; i++)
            {
                emptyslots.Add(i);
            }
            foreach (NbtCompound slot in invtag)
            {
                emptyslots.Remove(slot["Slot"].ByteValue);
            }
            return emptyslots;
        }

        protected override NbtCompound CreateChest(IEnumerable<long> mapids)
        {
            NbtList chestcontents = new NbtList("Items");
            byte slot = 0;
            foreach (var mapid in mapids)
            {
                chestcontents.Add(new NbtCompound
                {
                    new NbtString("id", "minecraft:filled_map"),
                    new NbtByte("Count", 1),
                    new NbtByte("Slot", slot),
                    new NbtShort("Damage", (short)mapid), // 1.12 support
                    new NbtCompound("tag") { new NbtInt("map", (int)mapid) } // 1.13+ support
                });
                slot++;
            }
            NbtCompound chest = new NbtCompound()
            {
                new NbtString("id", "minecraft:chest"),
                new NbtByte("Count", 1),
                new NbtCompound("tag") { new NbtCompound("BlockEntityTag") { chestcontents } }
            };
            return chest;
        }

        private string MapFileLocation(long mapid)
        {
            return Path.Combine(Folder, "data", $"map_{mapid}.dat");
        }

        private string PlayerFileLocation(string playerid)
        {
            return Path.Combine(Folder, "playerdata", $"{playerid}.dat");
        }
    }

    public class BedrockWorld : MinecraftWorld, IDisposable
    {
        private LevelDB.DB BedrockDB;

        public BedrockWorld(string folder) : base(folder)
        {
            BedrockDB = new LevelDB.DB(new LevelDB.Options() { CreateIfMissing = false }, Path.Combine(folder, "db"));
            Name = File.ReadAllText(Path.Combine(Folder, "levelname.txt"));
        }

        public override void AddMaps(Dictionary<long, Map> maps)
        {
            var batch = new LevelDB.WriteBatch();
            foreach (var map in maps)
            {
                NbtCompound mapfile = new NbtCompound("map")
                {
                    new NbtLong("mapId", map.Key),
                    new NbtLong("parentMapId", -1),
                    new NbtList("decorations", NbtTagType.Compound),
                    new NbtByte("fullyExplored", 1),
                    new NbtByte("scale", 4),
                    new NbtByte("dimension", 0),
                    new NbtShort("height", Map.MAP_HEIGHT),
                    new NbtShort("width", Map.MAP_WIDTH),
                    new NbtByte("unlimitedTracking", 0),
                    new NbtInt("xCenter", Int32.MaxValue),
                    new NbtInt("zCenter", Int32.MaxValue),
                    new NbtByte("mapLocked", 1),
                    new NbtByteArray("colors", map.Value.Colors)
                };
                NbtFile file = new NbtFile(mapfile);
                file.BigEndian = false;
                byte[] bytes = file.SaveToBuffer(NbtCompression.None);
                batch.Put(Encoding.Default.GetBytes($"map_{map.Key}"), bytes);
            }
            BedrockDB.Write(batch);
        }

        public override void RemoveMap(long mapid)
        {
            BedrockDB.Delete(Encoding.Default.GetBytes($"map_{mapid}"));
        }

        public override bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            // acquire the file this player is stored in, and the tag that represents said player
            byte[] playeridbytes;
            if (playerid == LOCAL_IDENTIFIER)
                playeridbytes = Encoding.Default.GetBytes("~local_player");
            else
                playeridbytes = Encoding.Default.GetBytes(playerid);
            byte[] playerdata = BedrockDB.Get(playeridbytes.ToArray());
            var file = new NbtFile();
            file.BigEndian = false;
            file.LoadFromBuffer(playerdata, 0, playerdata.Length, NbtCompression.None);
            var invtag = (NbtList)file.RootTag["Inventory"];

            var success = PutChestsInInventory(invtag, mapids);

            byte[] bytes = file.SaveToBuffer(NbtCompression.None);
            BedrockDB.Put(playeridbytes, bytes);

            return success;
        }

        public void Dispose()
        {
            BedrockDB?.Close();
            BedrockDB?.Dispose();
        }

        protected override Dictionary<long, Map> LoadMaps()
        {
            var maps = new Dictionary<long, Map>();
            foreach (var pair in BedrockDB)
            {
                string name = Encoding.Default.GetString(pair.Key);
                if (MapString(name, out long number))
                {
                    NbtFile nbtfile = new NbtFile();
                    nbtfile.BigEndian = false;
                    nbtfile.LoadFromBuffer(pair.Value, 0, pair.Value.Length, NbtCompression.AutoDetect);
                    maps.Add(number, new BedrockMap(nbtfile.RootTag["colors"].ByteArrayValue));
                }
            }
            return maps;
        }

        public override IEnumerable<string> GetPlayerIDs()
        {
            foreach (var pair in BedrockDB)
            {
                string name = Encoding.Default.GetString(pair.Key);
                if (UuidString(name))
                {
                    NbtFile nbtfile = new NbtFile();
                    nbtfile.BigEndian = false;
                    nbtfile.LoadFromBuffer(pair.Value, 0, pair.Value.Length, NbtCompression.AutoDetect);
                    if (nbtfile.RootTag["Inventory"] != null)
                        yield return name;
                }
            }
        }

        protected override IEnumerable<byte> GetFreeSlots(NbtList invtag)
        {
            List<byte> emptyslots = new List<byte>(35);
            for (byte i = 0; i < 35; i++)
            {
                emptyslots.Add(i);
            }
            foreach (NbtCompound slot in invtag)
            {
                if (slot["Count"].ByteValue > 0)
                    emptyslots.Remove(slot["Slot"].ByteValue);
            }
            return emptyslots;
        }

        protected override NbtCompound CreateChest(IEnumerable<long> mapids)
        {
            NbtList chestcontents = new NbtList("Items");
            byte slot = 0;
            foreach (var mapid in mapids)
            {
                chestcontents.Add(new NbtCompound
                {
                    new NbtString("Name", "minecraft:map"), // 1.6+ support
                    new NbtShort("id", 358), // 1.5 support
                    new NbtByte("Count", 1),
                    new NbtByte("Slot", slot),
                    new NbtCompound("tag") { new NbtLong("map_uuid", mapid)
                    }
                });
                slot++;
            }
            var chest = new NbtCompound()
            {
                new NbtString("Name", "minecraft:chest"), // 1.6+ support
                new NbtShort("id", 54), // 1.5 support
                new NbtByte("Count", 1),
                new NbtCompound("tag") { chestcontents }
            };
            return chest;
        }

        private static bool UuidString(string input)
        {
            return Regex.Match(input, @"^[0-f]{8}-[0-f]{4}-[0-f]{4}-[0-f]{4}-[0-f]{12}$").Success;
        }
    }

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
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);
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
