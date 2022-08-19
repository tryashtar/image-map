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

// map + ID, this is separate because we need to generate images without assigning them an ID yet
public class Map : ObservableObject
{
    private long _id;
    public long ID
    {
        get { return _id; }
        set { _id = value; OnPropertyChanged(); }
    }
    private MapData _data;
    public MapData Data
    {
        get { return _data; }
        set { _data = value; OnPropertyChanged(); }
    }
    public Map(long id, MapData data)
    {
        _id = id;
        _data = data;
    }
}

// doesn't contain all the map info (center, dimension, etc), just the pixels
public class MapData
{
    public Image<Rgba32> Image { get; }
    public Image<Rgba32> Original { get; }
    // makes it easier to render if we just have properties for this
    public ImageSource OriginalSource { get; }
    public ImageSource ImageSource { get; }
    public byte[] Colors { get; }
    public bool IsEmpty { get; }
    public MapData(Image<Rgba32> image, byte[] colors)
    {
        Image = image;
        Original = image;
        Colors = colors;
        ImageSource = new ImageSharpImageSource<Rgba32>(Image);
        ImageSource.Freeze();
        OriginalSource = ImageSource;
        IsEmpty = CheckIsEmpty();
    }
    public MapData(Image<Rgba32> image, Image<Rgba32> original, byte[] colors)
    {
        Image = image;
        Original = original;
        Colors = colors;
        ImageSource = new ImageSharpImageSource<Rgba32>(Image);
        ImageSource.Freeze();
        OriginalSource = new ImageSharpImageSource<Rgba32>(Original);
        OriginalSource.Freeze();
        IsEmpty = CheckIsEmpty();
    }
    private bool CheckIsEmpty()
    {
        for (int i = 0; i < Colors.Length; i++)
        {
            if (Colors[i] != 0)
                return false;
        }
        return true;
    }
}
