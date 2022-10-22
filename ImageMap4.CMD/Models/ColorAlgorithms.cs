using Colourful;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageMap4;

public interface IColorAlgorithm
{
    double Distance(Rgba32 c1, Rgba32 c2);
    private static readonly IColorConverter<RGBColor, LabColor> Converter = new ConverterBuilder().FromRGB(RGBWorkingSpaces.sRGB).ToLab(Illuminants.D50).Build();
    public static LabColor ToLab(Rgba32 color)
    {
        return Converter.Convert(RGBColor.FromRGB8Bit(color.R, color.G, color.B));
    }
}

public class SimpleAlgorithm : IColorAlgorithm
{
    // color distance algorithm I stole from https://stackoverflow.com/a/33782458
    // seems to work legitimately better and quicker than more sophisticated algorithms
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        long rmean = ((long)c1.R + c2.R) / 2;
        long r = (long)c1.R - c2.R;
        long g = (long)c1.G - c2.G;
        long b = (long)c1.B - c2.B;
        return (((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8);
    }
}

public class Ciede2000Algorithm : IColorAlgorithm
{
    private static readonly CIEDE2000ColorDifference Ciede2000 = new();
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        return Ciede2000.ComputeDifference(IColorAlgorithm.ToLab(c1), IColorAlgorithm.ToLab(c2));
    }
}

public class Cie76Algorithm : IColorAlgorithm
{
    private static readonly CIE76ColorDifference Ciede76 = new();
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        return Ciede76.ComputeDifference(IColorAlgorithm.ToLab(c1), IColorAlgorithm.ToLab(c2));
    }
}

public class CmcAlgorithm : IColorAlgorithm
{
    private static readonly CMCColorDifference Cmc = new(CMCColorDifferenceThreshold.Acceptability);
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        return Cmc.ComputeDifference(IColorAlgorithm.ToLab(c1), IColorAlgorithm.ToLab(c2));
    }
}

public class EuclideanAlgorithm : IColorAlgorithm
{
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        int redDifference = c1.R - c2.R;
        int greenDifference = c1.G - c2.G;
        int blueDifference = c1.B - c2.B;
        return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
    }
}

// https://bottosson.github.io/posts/oklab/
public class OkLabAlgorithm : IColorAlgorithm
{
    public double Distance(Rgba32 c1, Rgba32 c2)
    {
        var k1 = Convert(c1);
        var k2 = Convert(c2);
        double lDifference = k1.l - k2.l;
        double mDifference = k1.m - k2.m;
        double sDifference = k1.s - k2.s;
        return lDifference * lDifference + mDifference * mDifference + sDifference * sDifference;
    }

    private (double l, double m, double s) Convert(Rgba32 c)
    {
        double l = 0.4122214708d * c.R + 0.5363325363d * c.G + 0.0514459929d * c.B;
        double m = 0.2119034982d * c.R + 0.6806995451d * c.G + 0.1073969566d * c.B;
        double s = 0.0883024619d * c.R + 0.2817188376d * c.G + 0.6299787005d * c.B;

        l = Math.Pow(l, (double)1 / 3);
        m = Math.Pow(m, (double)1 / 3);
        s = Math.Pow(s, (double)1 / 3);

        return (
            0.2104542553d * l + 0.7936177850d * m - 0.0040720468d * s,
            1.9779984951d * l - 2.4285922050d * m + 0.4505937099d * s,
            0.0259040371d * l + 0.7827717662d * m - 0.8086757660d * s
        );
    }
}
