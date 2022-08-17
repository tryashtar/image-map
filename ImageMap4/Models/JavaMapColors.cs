using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageMap4;

public interface IMapColors
{
    Image<Rgba32> Decode(byte[] colors);
    ReadOnlyMemory<Color> GetPalette();
    byte[] EncodeColors(Image<Rgba32> image);
}

public static class JavaMapColors
{
    public static readonly MapColors Beta1p8 = new(
        new byte[] { 180, 220, 255, 220 },
        new Rgba32[] {
            Color.Transparent,
            new Rgba32(127, 178, 56),
            new Rgba32(247, 233, 163),
            new Rgba32(167, 167, 167),
            new Rgba32(255, 0, 0),
            new Rgba32(160, 160, 255),
            new Rgba32(167, 167, 167),
            new Rgba32(0, 124, 0),
            new Rgba32(255, 255, 255),
            new Rgba32(164, 168, 184),
            new Rgba32(183, 106, 47),
            new Rgba32(112, 112, 112),
            new Rgba32(64, 64, 255),
            new Rgba32(104, 83, 50)
    });
    public static readonly MapColors Snapshot13w42a = new(
        new byte[] { 176, 216, 250, 216 },
        Beta1p8,
        new Rgba32[] {
            new Rgba32(250, 250, 250),
            new Rgba32(212, 124, 50),
            new Rgba32(175, 74, 212),
            new Rgba32(100, 150, 212),
            new Rgba32(224, 224, 50),
            new Rgba32(124, 200, 24),
            new Rgba32(237, 124, 162),
            new Rgba32(74, 74, 74),
            new Rgba32(150, 150, 150),
            new Rgba32(74, 124, 150),
            new Rgba32(124, 62, 175),
            new Rgba32(50, 74, 175),
            new Rgba32(100, 74, 50),
            new Rgba32(100, 124, 50),
            new Rgba32(150, 50, 50),
            new Rgba32(24, 24, 24),
            new Rgba32(245, 233, 75),
            new Rgba32(90, 215, 209),
            new Rgba32(73, 125, 250),
            new Rgba32(0, 213, 57),
            new Rgba32(21, 20, 30),
            new Rgba32(110, 2, 0)
    });
    public static readonly MapColors Snapshot13w43a = new(
        new byte[] { 176, 216, 250, 132 },
        Snapshot13w42a,
        Array.Empty<Rgba32>()
    );
    public static readonly MapColors Release1p8 = new(
        new byte[] { 176, 216, 250, 132 },
        new Rgba32[] {
            Color.Transparent,
            new Rgba32(124, 175, 55),
            new Rgba32(242, 228, 160),
            new Rgba32(195, 195, 195),
            new Rgba32(250, 0, 0),
            new Rgba32(157, 157, 250),
            new Rgba32(164, 164, 164),
            new Rgba32(0, 122, 0),
            new Rgba32(250, 250, 250),
            new Rgba32(161, 165, 180),
            new Rgba32(148, 107, 75),
            new Rgba32(110, 110, 110),
            new Rgba32(63, 63, 250),
            new Rgba32(140, 117, 71),
            new Rgba32(250, 247, 240),
            new Rgba32(212, 124, 50),
            new Rgba32(175, 74, 212),
            new Rgba32(100, 150, 212),
            new Rgba32(224, 224, 50),
            new Rgba32(124, 199, 24),
            new Rgba32(236, 124, 161),
            new Rgba32(74, 74, 74),
            new Rgba32(149, 149, 149),
            new Rgba32(74, 124, 149),
            new Rgba32(124, 62, 174),
            new Rgba32(50, 74, 174),
            new Rgba32(100, 74, 50),
            new Rgba32(100, 124, 50),
            new Rgba32(149, 50, 50),
            new Rgba32(24, 24, 24),
            new Rgba32(245, 233, 75),
            new Rgba32(90, 215, 209),
            new Rgba32(73, 125, 250),
            new Rgba32(0, 213, 57),
            new Rgba32(126, 84, 48),
            new Rgba32(110, 2, 0)
    });
    public static readonly MapColors Release1p12 = new(
        new byte[] { 176, 216, 250, 132 },
        Release1p8,
        new Rgba32[] {
            new Rgba32(205, 174, 158),
            new Rgba32(156, 80, 35),
            new Rgba32(146, 85, 106),
            new Rgba32(110, 106, 135),
            new Rgba32(182, 130, 35),
            new Rgba32(101, 115, 52),
            new Rgba32(157, 75, 76),
            new Rgba32(56, 40, 34),
            new Rgba32(132, 105, 96),
            new Rgba32(85, 90, 90),
            new Rgba32(120, 72, 86),
            new Rgba32(74, 61, 90),
            new Rgba32(74, 49, 34),
            new Rgba32(74, 80, 41),
            new Rgba32(139, 59, 45),
            new Rgba32(36, 22, 16)
    });
    public static readonly MapColors Release1p16 = new(
        new byte[] { 176, 216, 250, 132 },
        Release1p12,
        new Rgba32[] {
            new Rgba32(185, 47, 48),
            new Rgba32(145, 62, 95),
            new Rgba32(90, 24, 28),
            new Rgba32(22, 124, 131),
            new Rgba32(57, 139, 137),
            new Rgba32(84, 43, 61),
            new Rgba32(20, 176, 130)
    });
    public static readonly MapColors Snapshot21w15a = new(
        new byte[] { 176, 216, 250, 132 },
        Release1p16,
        new Rgba32[] {
            new Rgba32(100, 100, 100),
            new Rgba32(216, 175, 147)
    });
    public static readonly MapColors Snapshot21w16a = new(
        new byte[] { 176, 216, 250, 132 },
        Snapshot21w15a,
        new Rgba32[] {
            new Rgba32(127, 167, 150)
    });
}

public class MapColors : IMapColors
{
    private readonly Dictionary<byte, Rgba32> ColorMap;
    private readonly Dictionary<Color, byte> ReverseColorMap;
    private readonly IEnumerable<Rgba32> BaseColors;
    private readonly byte[] Multipliers;
    public MapColors(byte[] multipliers, IEnumerable<Rgba32> colors)
    {
        Multipliers = multipliers;
        BaseColors = colors;
        ColorMap = new();
        ReverseColorMap = new();
        byte id = 0;
        foreach (var color in colors)
        {
            var alts = GetAlternateColors(color);
            foreach (var alt in alts)
            {
                ColorMap[id] = alt;
                if (!ReverseColorMap.ContainsKey(alt))
                    ReverseColorMap[alt] = id;
                id++;
            }
        }
    }
    public MapColors(byte[] multipliers, MapColors previous, IEnumerable<Rgba32> colors) : this(multipliers, previous.BaseColors.Concat(colors))
    {
    }

    public ReadOnlyMemory<Color> GetPalette()
    {
        return ColorMap.Values.Select(x => new Color(x)).ToArray();
    }

    public Image<Rgba32> Decode(byte[] colors)
    {
        byte[] pixels = new byte[128 * 128 * 4];
        for (int i = 0; i < colors.Length; i++)
        {
            if (ColorMap.TryGetValue(colors[i], out var color))
            {
                pixels[4 * i] = color.R;
                pixels[4 * i + 1] = color.G;
                pixels[4 * i + 2] = color.B;
                pixels[4 * i + 3] = color.A;
            }
        }
        return Image.LoadPixelData<Rgba32>(pixels, 128, 128);
    }

    public byte[] EncodeColors(Image<Rgba32> image)
    {
        byte[] result = new byte[image.Width * image.Height];
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var b = image[x, y];
                result[y * image.Height + x] = ReverseColorMap[b];
            }
        }
        return result;
    }

    private IEnumerable<Rgba32> GetAlternateColors(Rgba32 color)
    {
        return Multipliers.Select(x => Multiply(color, x));
    }

    private static Rgba32 Multiply(Rgba32 color, byte value)
    {
        return new Rgba32((byte)(color.R * value / 255), (byte)(color.G * value / 255), (byte)(color.B * value / 255), color.A);
    }
}
