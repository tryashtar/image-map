using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageMap4;
public class ImportViewModel : ObservableObject
{
    public ICommand RotateCommand { get; }
    public ICommand HorizontalFlipCommand { get; }
    public ICommand VerticalFlipCommand { get; }
    public ICommand SwitchImageCommand { get; }

    private bool _javaMode;
    public bool JavaMode
    {
        get { return _javaMode; }
        set { _javaMode = value; OnPropertyChanged(); }
    }

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

    public record ScalingOption(BitmapScalingMode? Mode, string Name);
    private ScalingOption _scaleChoice;
    public ScalingOption ScaleChoice
    {
        get { return _scaleChoice; }
        set { _scaleChoice = value; OnPropertyChanged(); }
    }
    public ReadOnlyCollection<ScalingOption> ScaleOptions { get; } = new List<ScalingOption>
    {
        new ScalingOption(null, "Automatic"),
        new ScalingOption(BitmapScalingMode.NearestNeighbor, "Pixel Art"),
        new ScalingOption(BitmapScalingMode.HighQuality, "Bicubic")
    }.AsReadOnly();

    public record ImagePair(Image<Rgba32> Image, ImageSource Source)
    {
        public ImagePair(Image<Rgba32> image) : this(image, new ImageSharpImageSource<Rgba32>(image)) { }
    }

    private readonly List<ImagePair> ImageQueue = new();
    public ReadOnlyCollection<ImagePair> Images => ImageQueue.AsReadOnly();
    private int CurrentIndex = 0;
    public ImagePair CurrentImage => ImageQueue.Count == 0 ? null : ImageQueue[CurrentIndex];

    private void MutatedCurrentImage()
    {
        // current image changed, need to recreate ImageSource
        ImageQueue[CurrentIndex] = new ImagePair(ImageQueue[CurrentIndex].Image);
        OnPropertyChanged(nameof(CurrentImage));
    }

    public ImportViewModel()
    {
        _stretchChoice = StretchOptions[Properties.Settings.Default.StretchChoice];
        _scaleChoice = ScaleOptions[Properties.Settings.Default.ScaleChoice];
        RotateCommand = new RelayCommand<float>(val =>
        {
            CurrentImage.Image.Mutate(x => x.Rotate(val));
            MutatedCurrentImage();
        });
        HorizontalFlipCommand = new RelayCommand(() =>
        {
            CurrentImage.Image.Mutate(x => x.Flip(FlipMode.Horizontal));
            MutatedCurrentImage();
        });
        VerticalFlipCommand = new RelayCommand(() =>
        {
            CurrentImage.Image.Mutate(x => x.Flip(FlipMode.Vertical));
            MutatedCurrentImage();
        });
        SwitchImageCommand = new RelayCommand<ImagePair>(pair =>
        {
            CurrentIndex = ImageQueue.FindIndex(x => x.Image == pair.Image);
            OnPropertyChanged(nameof(CurrentImage));
        });
    }

    public void AddImages(IEnumerable<string> paths)
    {
        var images = paths.Select(x => Image.Load<Rgba32>(x));
        ImageQueue.AddRange(images.Select(x => new ImagePair(x)));
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(Images));
    }
}
