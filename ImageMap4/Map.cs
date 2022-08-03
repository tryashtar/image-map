using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageMap4;

public class MapData
{
    public Image<Rgba32> Image { get; }
    public Image<Rgba32> Original { get; }
    public ImageSource OriginalSource { get; }
    public ImageSource ImageSource { get; }
    public byte[] Colors { get; }
    public MapData(Image<Rgba32> image, byte[] colors)
    {
        Image = image;
        Original = image;
        Colors = colors;
        ImageSource = new ImageSharpImageSource<Rgba32>(Image);
        OriginalSource = ImageSource;
    }
    public MapData(Image<Rgba32> image, Image<Rgba32> original, byte[] colors)
    {
        Image = image;
        Original = original;
        Colors = colors;
        ImageSource = new ImageSharpImageSource<Rgba32>(Image);
        OriginalSource = new ImageSharpImageSource<Rgba32>(Original);
    }
}
public record Map(long ID, MapData Data);
