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

    private bool _fillFrames;
    public bool FillFrames
    {
        get { return _fillFrames; }
        set { _fillFrames = value; OnPropertyChanged(); }
    }


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
