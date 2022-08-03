using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
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
    public event EventHandler<ImportSettings> OnConfirmed;

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

    public record StretchOption(string Name, Stretch Stretch, ResizeMode Mode);
    public StretchOption StretchChoice
    {
        get { return StretchOptions[Properties.Settings.Default.StretchChoice]; }
        set { Properties.Settings.Default.StretchChoice = StretchOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<StretchOption> StretchOptions { get; } = new List<StretchOption>
    {
        new StretchOption("Uniform", Stretch.Uniform, ResizeMode.Max),
        new StretchOption("Stretch", Stretch.Fill, ResizeMode.Stretch),
        new StretchOption("Crop", Stretch.UniformToFill, ResizeMode.Crop)
    }.AsReadOnly();

    public record ScalingOption(string Name, BitmapScalingMode Mode, IResampler Sampler);
    public ScalingOption ScaleChoice
    {
        get { return ScaleOptions[Properties.Settings.Default.ScaleChoice]; }
        set { Properties.Settings.Default.ScaleChoice = ScaleOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<ScalingOption> ScaleOptions { get; } = new List<ScalingOption>
    {
        new ScalingOption("Automatic", BitmapScalingMode.HighQuality, KnownResamplers.Bicubic),
        new ScalingOption("Pixel Art", BitmapScalingMode.NearestNeighbor, KnownResamplers.NearestNeighbor),
        new ScalingOption("Bicubic", BitmapScalingMode.HighQuality, KnownResamplers.Bicubic)
    }.AsReadOnly();

    public record DitherOption(string Name, IDither? Dither);
    public DitherOption DitherChoice
    {
        get { return DitherOptions[Properties.Settings.Default.DitherChoice]; }
        set { Properties.Settings.Default.DitherChoice = DitherOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<DitherOption> DitherOptions { get; } = new List<DitherOption>
    {
        new DitherOption("None", null),
        new DitherOption("Floyd Steinberg", KnownDitherings.FloydSteinberg),
        new DitherOption("Burks", KnownDitherings.Burks)
    }.AsReadOnly();

    public record AlgorithmOption(string Name, IColorAlgorithm Algorithm);
    public AlgorithmOption AlgorithmChoice
    {
        get { return AlgorithmOptions[Properties.Settings.Default.AlgorithmChoice]; }
        set { Properties.Settings.Default.AlgorithmChoice = AlgorithmOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<AlgorithmOption> AlgorithmOptions { get; } = new List<AlgorithmOption>
    {
        new AlgorithmOption("Good Fast", new SimpleAlgorithm()),
        new AlgorithmOption("Euclidean", new EuclideanAlgorithm()),
        new AlgorithmOption("CIEDE2000", new Ciede2000Algorithm()),
        new AlgorithmOption("CIE76", new Cie76Algorithm()),
        new AlgorithmOption("CMC", new CmcAlgorithm())
    }.AsReadOnly();

    public ObservableCollection<PreviewImage> ImageQueue { get; } = new();
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
            CloseIfDone();
        });
        DiscardAllCommand = new RelayCommand(() =>
        {
            ImageQueue.Clear();
            CurrentIndex = 0;
            OnPropertyChanged(nameof(CurrentImage));
            CloseIfDone();
        });
        ConfirmCommand = new RelayCommand(() =>
        {
            ConfirmImage(CurrentImage);
            ImageQueue.RemoveAt(CurrentIndex);
            if (CurrentIndex >= ImageQueue.Count)
                CurrentIndex--;
            OnPropertyChanged(nameof(CurrentImage));
            CloseIfDone();
        });
        ConfirmAllCommand = new RelayCommand(() =>
        {
            foreach (var item in ImageQueue)
            {
                ConfirmImage(item);
            }
            ImageQueue.Clear();
            CurrentIndex = 0;
            OnPropertyChanged(nameof(CurrentImage));
            CloseIfDone();
        });
        NavigateCommand = new RelayCommand<int>(x =>
        {
            CurrentIndex = ((CurrentIndex + x) % ImageQueue.Count + ImageQueue.Count) % ImageQueue.Count;
            OnPropertyChanged(nameof(CurrentImage));
        });
    }

    private void ConfirmImage(PreviewImage preview)
    {
        var settings = new ImportSettings(preview, GridWidth, GridHeight, ScaleChoice.Sampler, StretchChoice.Mode, new ProcessSettings(DitherChoice.Dither, AlgorithmChoice.Algorithm));
        OnConfirmed?.Invoke(this, settings);
    }

    private void CloseIfDone()
    {
        if (ImageQueue.Count == 0)
            OnClosed?.Invoke(this, EventArgs.Empty);
    }

    public void AddImages(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            ImageQueue.Add(new PreviewImage(path));
        }
        OnPropertyChanged(nameof(CurrentImage));
        HadMultiple = ImageQueue.Count > 1;
    }
}

public class PreviewImage : ObservableObject
{
    public string Source { get; }
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
        Source = path;
    }
}

public record ImportSettings(PreviewImage Preview, int Width, int Height, IResampler Sampler, ResizeMode ResizeMode, ProcessSettings ProcessSettings);

public record ProcessSettings(IDither? Dither, IColorAlgorithm Algorithm);
