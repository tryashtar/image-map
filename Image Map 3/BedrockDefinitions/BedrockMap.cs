using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
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
