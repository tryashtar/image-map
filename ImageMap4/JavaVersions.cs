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
    //byte[,][] Encode(Image<Rgba32> image, int width, int height);
}

public record struct Color(byte Red, byte Green, byte Blue, bool Visible = true)
{
    public Color Multiply(byte value)
    {
        return new Color((byte)(Red * value / 255), (byte)(Green * value / 255), (byte)(Blue * value / 255), Visible);
    }
    public static readonly Color Transparent = new Color(0, 0, 0, false);
}

public abstract class AbstractJavaVersion : IJavaVersion
{
    private readonly Dictionary<byte, Color> ColorMap;
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

    public Image<Rgba32> Decode(byte[] colors)
    {
        byte[] pixels = new byte[128 * 128 * 4];
        for (int i = 0; i < colors.Length; i++)
        {
            if (ColorMap.TryGetValue(colors[i], out var color))
            {
                pixels[4 * i] = color.Red;
                pixels[4 * i + 1] = color.Green;
                pixels[4 * i + 2] = color.Blue;
                pixels[4 * i + 3] = color.Visible ? (byte)255 : (byte)0;
            }
        }
        return Image.LoadPixelData<Rgba32>(pixels, 128, 128);
    }

    //public byte[,][] Encode(Image<Rgba32> image, int width, int height)
    //{
    //    var results = new byte[width, height][];
    //
    //}

    public abstract IEnumerable<Color> GetBaseColors();
    public abstract IEnumerable<Color> GetAlternateColors(Color color);

    protected IEnumerable<Color> AlternatesFromMultipliers(Color color, params byte[] multipliers)
    {
        return multipliers.Select(x => color.Multiply(x));
    }

    // sample a color from an image and pass it in here to get its "true" value before being scaled down to 250
    protected static IEnumerable<Color> FixShading(IEnumerable<Color> colors)
    {
        foreach (var color in colors)
        {
            yield return color.Multiply(250);
        }
    }
}

// beta 1.8+
public class JavaB1p8Version : AbstractJavaVersion
{
    // second and fourth colors are identical
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 180, 220, 255, 220);
    public static readonly ReadOnlyCollection<Color> Colors = new List<Color>
    {
        Color.Transparent,
        new Color(127, 178, 56),
        new Color(247, 233, 163),
        new Color(167, 167, 167),
        new Color(255, 0, 0),
        new Color(160, 160, 255),
        new Color(167, 167, 167),
        new Color(0, 124, 0),
        new Color(255, 255, 255),
        new Color(164, 168, 184),
        new Color(183, 106, 47),
        new Color(112, 112, 112),
        new Color(64, 64, 255),
        new Color(104, 83, 50)
    }.AsReadOnly();
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "Beta 1.8+";
}

// 13w42a+
public class Java1p7SnapshotVersion : AbstractJavaVersion
{
    // second and fourth colors are identical
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 216);
    public static readonly ReadOnlyCollection<Color> Colors;
    static Java1p7SnapshotVersion()
    {
        var colors = new List<Color>(JavaB1p8Version.Colors);
        colors.AddRange(FixShading(new List<Color>
        {
           new Color(250, 250, 250),
           new Color(212, 124, 50),
           new Color(175, 74, 212),
           new Color(100, 150, 212),
           new Color(224, 224, 50),
           new Color(124, 200, 24),
           new Color(237, 124, 162),
           new Color(74, 74, 74),
           new Color(150, 150, 150),
           new Color(74, 124, 150),
           new Color(124, 62, 175),
           new Color(50, 74, 175),
           new Color(100, 74, 50),
           new Color(100, 124, 50),
           new Color(150, 50, 50),
           new Color(24, 24, 24),
           new Color(245, 233, 75),
           new Color(90, 215, 209),
           new Color(73, 125, 250),
           new Color(0, 213, 57),
           new Color(21, 20, 30),
           new Color(110, 2, 0)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "13w42a+";
}

// 13w43a+
public class Java1p7Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Color> GetBaseColors() => Java1p7SnapshotVersion.Colors;
    public override string ToString() => "1.7+";
}

// 1.8.1-pre1+
public class Java1p8Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Color> Colors = FixShading(new List<Color>
    {
        Color.Transparent,
        new Color(124, 175, 55),
        new Color(242, 228, 160),
        new Color(195, 195, 195),
        new Color(250, 0, 0),
        new Color(157, 157, 250),
        new Color(164, 164, 164),
        new Color(0, 122, 0),
        new Color(250, 250, 250),
        new Color(161, 165, 180),
        new Color(148, 107, 75),
        new Color(110, 110, 110),
        new Color(63, 63, 250),
        new Color(140, 117, 71),
        new Color(250, 247, 240),
        new Color(212, 124, 50),
        new Color(175, 74, 212),
        new Color(100, 150, 212),
        new Color(224, 224, 50),
        new Color(124, 199, 24),
        new Color(236, 124, 161),
        new Color(74, 74, 74),
        new Color(149, 149, 149),
        new Color(74, 124, 149),
        new Color(124, 62, 174),
        new Color(50, 74, 174),
        new Color(100, 74, 50),
        new Color(100, 124, 50),
        new Color(149, 50, 50),
        new Color(24, 24, 24),
        new Color(245, 233, 75),
        new Color(90, 215, 209),
        new Color(73, 125, 250),
        new Color(0, 213, 57),
        new Color(126, 84, 48),
        new Color(110, 2, 0)
    }).ToList().AsReadOnly();
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "1.8+";
}

// 17w17a+
public class Java1p12Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Color> Colors;
    static Java1p12Version()
    {
        var colors = new List<Color>(Java1p8Version.Colors);
        colors.AddRange(FixShading(new List<Color>
        {
            new Color(205, 174, 158),
            new Color(156, 80, 35),
            new Color(146, 85, 106),
            new Color(110, 106, 135),
            new Color(182, 130, 35),
            new Color(101, 115, 52),
            new Color(157, 75, 76),
            new Color(56, 40, 34),
            new Color(132, 105, 96),
            new Color(85, 90, 90),
            new Color(120, 72, 86),
            new Color(74, 61, 90),
            new Color(74, 49, 34),
            new Color(74, 80, 41),
            new Color(139, 59, 45),
            new Color(36, 22, 16)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "1.12+";
}

// 17w47a+
public class Java1p13Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Color> GetBaseColors() => Java1p12Version.Colors;
    public override string ToString() => "1.13+";
}

// 19w02a+
public class Java1p14Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public override IEnumerable<Color> GetBaseColors() => Java1p12Version.Colors;
    public override string ToString() => "1.14+";
}

// 1.16 pre-6+
public class Java1p16Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Color> Colors;
    static Java1p16Version()
    {
        var colors = new List<Color>(Java1p12Version.Colors);
        colors.AddRange(FixShading(new List<Color>
        {
            new Color(185, 47, 48),
            new Color(145, 62, 95),
            new Color(90, 24, 28),
            new Color(22, 124, 131),
            new Color(57, 139, 137),
            new Color(84, 43, 61),
            new Color(20, 176, 130)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "1.16+";
}

// 21w15a+
public class Java1p17SnapshotVersion : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Color> Colors;
    static Java1p17SnapshotVersion()
    {
        var colors = new List<Color>(Java1p16Version.Colors);
        colors.AddRange(FixShading(new List<Color>
        {
            new Color(100, 100, 100),
            new Color(216, 175, 147)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "21w15a";
}

// 21w16a+
public class Java1p17Version : AbstractJavaVersion
{
    public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, 176, 216, 250, 132);
    public static readonly ReadOnlyCollection<Color> Colors;
    static Java1p17Version()
    {
        var colors = new List<Color>(Java1p17SnapshotVersion.Colors);
        colors.AddRange(FixShading(new List<Color>
        {
            new Color(127, 167, 150)
        }));
        Colors = colors.AsReadOnly();
    }
    public override IEnumerable<Color> GetBaseColors() => Colors;
    public override string ToString() => "1.17+";
}
