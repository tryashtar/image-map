using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageMap4;
public class ImportViewModel : ObservableObject
{
    private int _gridWidth = 1;
    public int GridWidth
    {
        get { return _gridWidth; }
        set { _gridWidth = value; OnPropertyChanged(); }
    }

    private int _gridHeight = 1;
    public int GridHeight
    {
        get { return _gridHeight; }
        set { _gridHeight = value; OnPropertyChanged(); }
    }

    public record StretchOption(Stretch Stretch, string Name);
    private StretchOption _stretchChoice;
    public StretchOption StretchChoice
    {
        get { return _stretchChoice; }
        set { _stretchChoice = value; OnPropertyChanged(); }
    }
    public ReadOnlyCollection<StretchOption> StretchOptions { get; } = new List<StretchOption>
    {
        new StretchOption(Stretch.Uniform, "Uniform"),
        new StretchOption(Stretch.Fill, "Stretch"),
        new StretchOption(Stretch.UniformToFill, "Crop")
    }.AsReadOnly();


    private readonly List<Image<Rgba32>> ImageQueue = new();
    private int CurrentIndex = 0;

    public Image<Rgba32>? CurrentImage => ImageQueue.Count == 0 ? null : ImageQueue[CurrentIndex];
    public ImageSource? CurrentSource => ImageQueue.Count == 0 ? null : new ImageSharpImageSource<Rgba32>(ImageQueue[CurrentIndex]);
    public void AddImages(IEnumerable<string> paths)
    {
        var images = paths.Select(x => Image.Load<Rgba32>(x));
        ImageQueue.AddRange(images);
        OnPropertyChanged(nameof(CurrentSource));
    }
}
