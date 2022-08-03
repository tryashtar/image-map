using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMap4;

public interface IJavaVersion
{
    Image<Rgba32> Decode(byte[] colors);
    ReadOnlyMemory<Color> GetPalette();
    //byte[,][] Encode(Image<Rgba32> image, int width, int height);
}

public abstract class AbstractJavaVersion : IJavaVersion
{
    private readonly Dictionary<byte, Rgba32> ColorMap;
    public AbstractJavaVersion()
    {
        var colors = GetBaseColors();
        ColorMap = new();
        byte id = 0;
        foreach (var color in colors)
        {
            var alts = GetAlternateColors(color);
            foreach (var alt in alts)
            {
                ColorMap[id] = alt;
                id++;
            }
        }
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

    //public byte[,][] Encode(Image<Rgba32> image, int width, int height)
    //{
    //    var results = new byte[width, height][];
    //
    //}

    public abstract IEnumerable<Rgba32> GetBaseColors();
    public abstract IEnumerable<Rgba32> GetAlternateColors(Rgba32 color);

    protected static IEnumerable<Rgba32> AlternatesFromMultipliers(Rgba32 color, params byte[] multipliers)
    {
        return multipliers.Select(x => Multiply(color, x));
    }

    protected static Rgba32 Multiply(Rgba32 color, byte value)
    {
        return new Rgba32((byte)(color.R * value / 255), (byte)(color.G * value / 255), (byte)(color.B * value / 255), color.A);
    }

    // sample a color from an image and pass it in here to get its "true" value before being scaled down to 250
    protected static IEnumerable<Rgba32> FixShading(IEnumerable<Rgba32> colors)
    {
        foreach (var color in colors)
        {
            yield return Multiply(color, 250);
        }
    }
}

// beta 1.8+
public class JavaB1p8Version : AbstractJavaVersion
{
    // second and fourth colors are identical
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 180, 220, 255, 220);
    public static readonly ReadOnlyCollection<Rgba32> Colors = new List<Rgba32>
    {
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
    }.AsReadOnly();
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "Beta 1.8+";
}

// 13w42a+
public class Java1p7SnapshotVersion : AbstractJavaVersion
{
    // second and fourth colors are identical
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 216);
    public static readonly ReadOnlyCollection<Rgba32> Colors;
    static Java1p7SnapshotVersion()
    {
        var colors = new List<Rgba32>(JavaB1p8Version.Colors);
        colors.AddRange(FixShading(new List<Rgba32>
        {
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
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "13w42a+";
}

// 13w43a+
public class Java1p7Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Rgba32> GetBaseColors() => Java1p7SnapshotVersion.Colors;
    public override string ToString() => "1.7+";
}

// 1.8.1-pre1+
public class Java1p8Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Rgba32> Colors = FixShading(new List<Rgba32>
    {
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
    }).ToList().AsReadOnly();
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "1.8+";
}

// 17w17a+
public class Java1p12Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Rgba32> Colors;
    static Java1p12Version()
    {
        var colors = new List<Rgba32>(Java1p8Version.Colors);
        colors.AddRange(FixShading(new List<Rgba32>
        {
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
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "1.12+";
}

// 17w47a+
public class Java1p13Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Rgba32> GetBaseColors() => Java1p12Version.Colors;
    public override string ToString() => "1.13+";
}

// 19w02a+
public class Java1p14Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Rgba32> GetBaseColors() => Java1p12Version.Colors;
    public override string ToString() => "1.14+";
}

// 1.16 pre-6+
public class Java1p16Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Rgba32> Colors;
    static Java1p16Version()
    {
        var colors = new List<Rgba32>(Java1p12Version.Colors);
        colors.AddRange(FixShading(new List<Rgba32>
        {
            new Rgba32(185, 47, 48),
            new Rgba32(145, 62, 95),
            new Rgba32(90, 24, 28),
            new Rgba32(22, 124, 131),
            new Rgba32(57, 139, 137),
            new Rgba32(84, 43, 61),
            new Rgba32(20, 176, 130)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "1.16+";
}

// 21w15a+
public class Java1p17SnapshotVersion : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Rgba32> Colors;
    static Java1p17SnapshotVersion()
    {
        var colors = new List<Rgba32>(Java1p16Version.Colors);
        colors.AddRange(FixShading(new List<Rgba32>
        {
            new Rgba32(100, 100, 100),
            new Rgba32(216, 175, 147)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "21w15a";
}

// 21w16a+
public class Java1p17Version : AbstractJavaVersion
{
    public override IEnumerable<Rgba32> GetAlternateColors(Rgba32 color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Rgba32> Colors;
    static Java1p17Version()
    {
        var colors = new List<Rgba32>(Java1p17SnapshotVersion.Colors);
        colors.AddRange(FixShading(new List<Rgba32>
        {
            new Rgba32(127, 167, 150)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Rgba32> GetBaseColors() => Colors;
    public override string ToString() => "1.17+";
}
