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
using System.Windows.Media.Imaging;

namespace ImageMap4;
public class ImportViewModel : ObservableObject
{
    public ICommand RotateCommand { get; }
    public ICommand HorizontalFlipCommand { get; }
    public ICommand VerticalFlipCommand { get; }
    public ICommand SwitchImageCommand { get; }
    public ICommand DiscardCommand { get; }
    public ICommand DiscardAllCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand ConfirmAllCommand { get; }
    public ICommand NavigateCommand { get; }
    public event EventHandler OnClosed;

    private bool _hadMultiple;
    public bool HadMultiple
    {
        get { return _hadMultiple; }
        set { _hadMultiple = value; OnPropertyChanged(); }
    }

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
    public StretchOption StretchChoice
    {
        get { return StretchOptions[Properties.Settings.Default.StretchChoice]; }
        set { Properties.Settings.Default.StretchChoice = StretchOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<StretchOption> StretchOptions { get; } = new List<StretchOption>
    {
        new StretchOption(Stretch.Uniform, "Uniform"),
        new StretchOption(Stretch.Fill, "Stretch"),
        new StretchOption(Stretch.UniformToFill, "Crop")
    }.AsReadOnly();

    public record ScalingOption(BitmapScalingMode? Mode, string Name);
    public ScalingOption ScaleChoice
    {
        get { return ScaleOptions[Properties.Settings.Default.ScaleChoice]; }
        set { Properties.Settings.Default.ScaleChoice = ScaleOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<ScalingOption> ScaleOptions { get; } = new List<ScalingOption>
    {
        new ScalingOption(null, "Automatic"),
        new ScalingOption(BitmapScalingMode.NearestNeighbor, "Pixel Art"),
        new ScalingOption(BitmapScalingMode.HighQuality, "Bicubic")
    }.AsReadOnly();

    private readonly List<PreviewImage> ImageQueue = new();
    public ReadOnlyCollection<PreviewImage> Images => ImageQueue.AsReadOnly();
    private int CurrentIndex = 0;
    public PreviewImage CurrentImage => ImageQueue.Count == 0 ? null : ImageQueue[CurrentIndex];

    public ImportViewModel()
    {
        RotateCommand = new RelayCommand<float>(val =>
        {
            CurrentImage.Rotation = (CurrentImage.Rotation + val) % 360;
        });
        HorizontalFlipCommand = new RelayCommand(() =>
        {
            CurrentImage.ScaleX *= -1;
        });
        VerticalFlipCommand = new RelayCommand(() =>
        {
            CurrentImage.ScaleY *= -1;
        });
        SwitchImageCommand = new RelayCommand<PreviewImage>(preview =>
        {
            CurrentIndex = ImageQueue.IndexOf(preview);
            OnPropertyChanged(nameof(CurrentImage));
        });
        DiscardCommand = new RelayCommand(() =>
        {
            ImageQueue.RemoveAt(CurrentIndex);
            if (CurrentIndex >= ImageQueue.Count)
                CurrentIndex--;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(Images));
            CloseIfDone();
        });
        DiscardAllCommand = new RelayCommand(() =>
        {
            ImageQueue.Clear();
            CurrentIndex = 0;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(Images));
            CloseIfDone();
        });
        NavigateCommand = new RelayCommand<int>(x =>
        {
            CurrentIndex = ((CurrentIndex + x) % ImageQueue.Count + ImageQueue.Count) % ImageQueue.Count;
            OnPropertyChanged(nameof(CurrentImage));
        });
    }

    private void CloseIfDone()
    {
        if (ImageQueue.Count == 0)
            OnClosed?.Invoke(this, EventArgs.Empty);
    }

    public void AddImages(IEnumerable<string> paths)
    {
        ImageQueue.AddRange(paths.Select(x => new PreviewImage(x)));
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(Images));
        HadMultiple = ImageQueue.Count > 1;
    }
}

public class PreviewImage : ObservableObject
{
    public ImageSource Source { get; }
    private double _rotation;
    public double Rotation
    {
        get { return _rotation; }
        set { _rotation = value; OnPropertyChanged(); OnPropertyChanged(nameof(Source)); }
    }
    private double _scaleX = 1;
    public double ScaleX
    {
        get { return _scaleX; }
        set { _scaleX = value; OnPropertyChanged(); OnPropertyChanged(nameof(Source)); }
    }
    private double _scaleY = 1;
    public double ScaleY
    {
        get { return _scaleY; }
        set { _scaleY = value; OnPropertyChanged(); OnPropertyChanged(nameof(Source)); }
    }

    public PreviewImage(string path)
    {
        Source = new BitmapImage(new Uri(path));
    }
}
