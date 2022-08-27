using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

public class MapData
{
    public Image<Rgba32> Image { get; }
    public Image<Rgba32> Original { get; }
    public byte[] Colors { get; }
    public bool IsEmpty { get; }
    // save FullData when loading from world, that way if we call AddMaps on the values from GetMaps, we won't lose any tags
    public NbtCompound? FullData { get; }
    public MapData(Image<Rgba32> image, byte[] colors, NbtCompound? fullData = null)
    {
        Image = image;
        Original = image;
        Colors = colors;
        IsEmpty = CheckIsEmpty();
        FullData = fullData;
    }
    public MapData(Image<Rgba32> image, Image<Rgba32> original, byte[] colors)
    {
        Image = image;
        Original = original;
        Colors = colors;
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

public class PendingSource
{
    public Remakable<Image<Rgba32>> Image { get; }
    public string Name { get; }

    public PendingSource(Func<Image<Rgba32>> image, string name)
    {
        Image = new(image, IsDisposed);
        Name = name;
    }

    private static bool IsDisposed(Image<Rgba32> img)
    {
        // eek!
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var field = typeof(Image).GetField("isDisposed", flags);
        return (bool)field.GetValue(img);
    }

    public static PendingSource FromPath(string path)
    {
        return new PendingSource(() => SixLabors.ImageSharp.Image.Load<Rgba32>(path), Path.GetFileName(path));
    }
}

public class Remakable<T> where T : class, IDisposable
{
    private T? _value = null;
    public T Value
    {
        get
        {
            if (_value == null || DisposedCheck(_value))
                _value = Getter();
            return _value;
        }
    }
    private readonly Func<T> Getter;
    private readonly Func<T, bool> DisposedCheck;
    public Remakable(Func<T> getter, Func<T, bool> disposed_check)
    {
        Getter = getter;
        DisposedCheck = disposed_check;
    }
}

public class PreviewImage : ObservableObject
{
    public PendingSource Source { get; }
    private double _rotation;
    public double Rotation
    {
        get { return _rotation; }
        set { _rotation = value; OnPropertyChanged(); }
    }
    private double _scaleX = 1;
    public double ScaleX
    {
        get { return _scaleX; }
        set { _scaleX = value; OnPropertyChanged(); }
    }
    private double _scaleY = 1;
    public double ScaleY
    {
        get { return _scaleY; }
        set { _scaleY = value; OnPropertyChanged(); }
    }

    public PreviewImage(PendingSource source)
    {
        Source = source;
    }
}
