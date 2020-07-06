using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public interface IColorAlgorithm
    {
        double Distance(Color c1, Color c2);
    }

    public class SimpleAlgorithm : IColorAlgorithm
    {
        public static readonly SimpleAlgorithm Instance = new SimpleAlgorithm();
        private SimpleAlgorithm() { }

        // color distance algorithm I stole from https://stackoverflow.com/a/33782458
        // seems to work legitimately better and quicker than more sophisticated algorithms
        public double Distance(Color c1, Color c2)
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
        private static readonly CIEDE2000ColorDifference Ciede2000 = new CIEDE2000ColorDifference();
        private static readonly ColourfulConverter Converter = new ColourfulConverter();
        public static readonly Ciede2000Algorithm Instance = new Ciede2000Algorithm();
        private Ciede2000Algorithm() { }
        public double Distance(Color c1, Color c2)
        {
            return Ciede2000.ComputeDifference(Converter.ToLab(new RGBColor(c1)), Converter.ToLab(new RGBColor(c2)));
        }
    }

    public class Cie76Algorithm : IColorAlgorithm
    {
        private static readonly CIE76ColorDifference Ciede76 = new CIE76ColorDifference();
        private static readonly ColourfulConverter Converter = new ColourfulConverter();
        public static readonly Cie76Algorithm Instance = new Cie76Algorithm();
        private Cie76Algorithm() { }
        public double Distance(Color c1, Color c2)
        {
            return Ciede76.ComputeDifference(Converter.ToLab(new RGBColor(c1)), Converter.ToLab(new RGBColor(c2)));
        }
    }

    public class CmcAlgorithm : IColorAlgorithm
    {
        private static readonly CMCColorDifference Cmc = new CMCColorDifference(CMCColorDifferenceThreshold.Acceptability);
        private static readonly ColourfulConverter Converter = new ColourfulConverter();
        public static readonly CmcAlgorithm Instance = new CmcAlgorithm();
        private CmcAlgorithm() { }
        public double Distance(Color c1, Color c2)
        {
            return Cmc.ComputeDifference(Converter.ToLab(new RGBColor(c1)), Converter.ToLab(new RGBColor(c2)));
        }
    }

    public class EuclideanAlgorithm : IColorAlgorithm
    {
        public static readonly EuclideanAlgorithm Instance = new EuclideanAlgorithm();
        private EuclideanAlgorithm() { }
        public double Distance(Color c1, Color c2)
        {
            int redDifference = c1.R - c2.R;
            int greenDifference = c1.G - c2.G;
            int blueDifference = c1.B - c2.B;
            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }
    }
}
