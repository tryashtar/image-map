using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageMap
{
    public interface IColorMapping
    {
        byte ColorToByte(Color input);
        bool TryColorToByte(Color input, out byte output);
        Color ByteToColor(byte input);
        bool TryByteToColor(byte input, out Color output);
        IEnumerable<Color> GetAllColors();
    }

    public abstract class JavaMapping : IColorMapping
    {
        private readonly Dictionary<Color, byte> ColorMap;
        private readonly Dictionary<byte, Color> ReverseColorMap;
        public JavaMapping()
        {
            var colors = GetBaseColors();
            ColorMap = new Dictionary<Color, byte>();
            byte id = 0;
            foreach (var color in colors)
            {
                var alts = GetAlternateColors(color);
                foreach (var alt in alts)
                {
                    ColorMap[alt] = id;
                    id++;
                }
            }
            ReverseColorMap = ColorMap.ToDictionary(x => x.Value, x => x.Key);
        }

        public abstract IEnumerable<Color> GetBaseColors();
        public abstract IEnumerable<Color> GetAlternateColors(Color color);

        protected IEnumerable<Color> AlternatesFromMultipliers(Color color, IEnumerable<int> multipliers)
        {
            return multipliers.Select(x => Color.FromArgb(color.A, color.R * x / 255, color.G * x / 255, color.B * x / 255));
        }

        // sample a color from an image and pass it in here to get its "true" value before being scaled down to 250
        protected static IEnumerable<Color> FixShading(IEnumerable<Color> colors)
        {
            foreach (var color in colors)
            {
                yield return Color.FromArgb(color.A, color.R * 255 / 250, color.G * 255 / 250, color.B * 255 / 250);
            }
        }

        public byte ColorToByte(Color input)
        {
            return ColorMap[input];
        }

        public bool TryColorToByte(Color input, out byte output)
        {
            return ColorMap.TryGetValue(input, out output);
        }

        public Color ByteToColor(byte input)
        {
            return ReverseColorMap[input];
        }

        public bool TryByteToColor(byte input, out Color output)
        {
            return ReverseColorMap.TryGetValue(input, out output);
        }

        public IEnumerable<Color> GetAllColors() => ColorMap.Keys;
    }

    // beta 1.8+
    public class JavaOldMapping : JavaMapping
    {
        public static JavaOldMapping Instance = new JavaOldMapping();
        private JavaOldMapping() { }

        // second and fourth colors are identical
        public static readonly int[] ColorMultipliers = new int[] { 180, 220, 255, 220 };
        public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, ColorMultipliers);

        public override IEnumerable<Color> GetBaseColors()
        {
            return new List<Color>
            {
                Color.Transparent,
                Color.FromArgb(127, 178, 56),
                Color.FromArgb(247, 233, 163),
                Color.FromArgb(167, 167, 167),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(160, 160, 255),
                Color.FromArgb(167, 167, 167),
                Color.FromArgb(0, 124, 0),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(164, 168, 184),
                Color.FromArgb(183, 106, 47),
                Color.FromArgb(112, 112, 112),
                Color.FromArgb(64, 64, 255),
                Color.FromArgb(104, 83, 50)
            };
        }
    }

    // 13w42a+
    public class Java1p7SnapshotMapping : JavaMapping
    {
        public static Java1p7SnapshotMapping Instance = new Java1p7SnapshotMapping();
        private Java1p7SnapshotMapping() { }

        // second and fourth colors are identical
        public static readonly int[] ColorMultipliers = new int[] { 176, 216, 250, 216 };
        public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, ColorMultipliers);

        public override IEnumerable<Color> GetBaseColors()
        {
            var old_colors = JavaOldMapping.Instance.GetBaseColors();
            var new_colors = FixShading(new List<Color>
            {
               Color.FromArgb(250, 250, 250),
               Color.FromArgb(212, 124, 50),
               Color.FromArgb(175, 74, 212),
               Color.FromArgb(100, 150, 212),
               Color.FromArgb(224, 224, 50),
               Color.FromArgb(124, 200, 24),
               Color.FromArgb(237, 124, 162),
               Color.FromArgb(74, 74, 74),
               Color.FromArgb(150, 150, 150),
               Color.FromArgb(74, 124, 150),
               Color.FromArgb(124, 62, 175),
               Color.FromArgb(50, 74, 175),
               Color.FromArgb(100, 74, 50),
               Color.FromArgb(100, 124, 50),
               Color.FromArgb(150, 50, 50),
               Color.FromArgb(24, 24, 24),
               Color.FromArgb(245, 233, 75),
               Color.FromArgb(90, 215, 209),
               Color.FromArgb(73, 125, 250),
               Color.FromArgb(0, 213, 57),
               Color.FromArgb(21, 20, 30),
               Color.FromArgb(110, 2, 0)
            });
            return old_colors.Concat(new_colors);
        }
    }

    // 13w43a+
    public class Java1p7Mapping : JavaMapping
    {
        public static Java1p7Mapping Instance = new Java1p7Mapping();
        private Java1p7Mapping() { }

        public static readonly int[] ColorMultipliers = new int[] { 176, 216, 250, 132 };
        public override IEnumerable<Color> GetAlternateColors(Color color) => AlternatesFromMultipliers(color, ColorMultipliers);

        public override IEnumerable<Color> GetBaseColors() => Java1p7SnapshotMapping.Instance.GetBaseColors();
    }

    // 1.8.1-pre1+
    public class Java1p8Mapping : JavaMapping
    {
        public static Java1p8Mapping Instance = new Java1p8Mapping();
        private Java1p8Mapping() { }

        public override IEnumerable<Color> GetAlternateColors(Color color) => Java1p7Mapping.Instance.GetAlternateColors(color);

        public override IEnumerable<Color> GetBaseColors()
        {
            // most colors were changed, ignore previous versions
            return FixShading(new List<Color>
            {
                Color.FromArgb(124, 175, 55),
                Color.FromArgb(242, 228, 160),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(250, 0, 0),
                Color.FromArgb(157, 157, 250),
                Color.FromArgb(164, 164, 164),
                Color.FromArgb(0, 122, 0),
                Color.FromArgb(250, 250, 250),
                Color.FromArgb(161, 165, 180),
                Color.FromArgb(148, 107, 75),
                Color.FromArgb(110, 110, 110),
                Color.FromArgb(63, 63, 250),
                Color.FromArgb(140, 117, 71),
                Color.FromArgb(250, 247, 240),
                Color.FromArgb(212, 124, 50),
                Color.FromArgb(175, 74, 212),
                Color.FromArgb(100, 150, 212),
                Color.FromArgb(224, 224, 50),
                Color.FromArgb(124, 199, 24),
                Color.FromArgb(236, 124, 161),
                Color.FromArgb(74, 74, 74),
                Color.FromArgb(149, 149, 149),
                Color.FromArgb(74, 124, 149),
                Color.FromArgb(124, 62, 174),
                Color.FromArgb(50, 74, 255),
                Color.FromArgb(100, 74, 50),
                Color.FromArgb(100, 124, 50),
                Color.FromArgb(149, 50, 50),
                Color.FromArgb(24, 24, 24),
                Color.FromArgb(245, 233, 75),
                Color.FromArgb(90, 215, 209),
                Color.FromArgb(73, 125, 250),
                Color.FromArgb(0, 213, 57),
                Color.FromArgb(126, 84, 48),
                Color.FromArgb(110, 2, 0),
            });
        }
    }

    // 17w17a+
    public class Java1p12Mapping : JavaMapping
    {
        public static Java1p12Mapping Instance = new Java1p12Mapping();
        private Java1p12Mapping() { }

        public override IEnumerable<Color> GetAlternateColors(Color color) => Java1p7Mapping.Instance.GetAlternateColors(color);

        public override IEnumerable<Color> GetBaseColors()
        {
            var old_colors = Java1p8Mapping.Instance.GetBaseColors();
            var new_colors = FixShading(new List<Color>
            {
                Color.FromArgb(205, 174, 158),
                Color.FromArgb(156, 80, 35),
                Color.FromArgb(146, 85, 106),
                Color.FromArgb(110, 106, 135),
                Color.FromArgb(182, 130, 35),
                Color.FromArgb(101, 115, 52),
                Color.FromArgb(157, 75, 76),
                Color.FromArgb(56, 40, 34),
                Color.FromArgb(132, 105, 96),
                Color.FromArgb(85, 90, 90),
                Color.FromArgb(120, 72, 86),
                Color.FromArgb(74, 61, 90),
                Color.FromArgb(74, 49, 34),
                Color.FromArgb(74, 80, 41),
                Color.FromArgb(139, 59, 45),
                Color.FromArgb(36, 22, 16)
            });
            return old_colors.Concat(new_colors);
        }
    }

    // 1.16 pre-6+
    public class Java1p16Mapping : JavaMapping
    {
        public static Java1p16Mapping Instance = new Java1p16Mapping();
        private Java1p16Mapping() { }
        public override IEnumerable<Color> GetAlternateColors(Color color) => Java1p8Mapping.Instance.GetAlternateColors(color);
        public override IEnumerable<Color> GetBaseColors()
        {
            var old_colors = Java1p12Mapping.Instance.GetBaseColors();
            var new_colors = FixShading(new List<Color>
            {
                Color.FromArgb(185, 47, 48),
                Color.FromArgb(145, 62, 95),
                Color.FromArgb(90, 24, 28),
                Color.FromArgb(22, 124, 131),
                Color.FromArgb(57, 139, 137),
                Color.FromArgb(84, 43, 61),
                Color.FromArgb(20, 176, 130)
            });
            return old_colors.Concat(new_colors);
        }
    }
}
