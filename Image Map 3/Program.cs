using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
#if DEBUG
            CreateSupportedColorsImage();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TheForm());
        }

        private static void CreateSupportedColorsImage()
        {
            var versions = new AbstractJavaVersion[] {
                JavaB1p8Version.Instance,
                Java1p7SnapshotVersion.Instance,
                Java1p7Version.Instance,
                Java1p8Version.Instance,
                Java1p12Version.Instance,
                Java1p16Version.Instance,
                Java1p17Version.Instance,
            };
            foreach (var version in versions)
            {
                var colors = version.GetBaseColors().ToArray();
                var alternate_count = version.GetAlternateColors(colors.First()).Count();
                var image = new Bitmap(colors.Length, alternate_count);
                for (int x = 0; x < colors.Length; x++)
                {
                    var alts = version.GetAlternateColors(colors[x]).ToArray();
                    for (int y = 0; y < alts.Length; y++)
                    {
                        image.SetPixel(x, y, alts[y]);
                    }
                }
                var scaled_image = new Bitmap(image.Width * 20, image.Height * 20);
                using (var g = Graphics.FromImage(scaled_image))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(image, 0, 0, scaled_image.Width, scaled_image.Height);
                }
                scaled_image.Save($"scaled_{version}_palette.png");
            }
        }
    }
}
