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
using System.Reflection;
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
    public ICommand ChangeBackgroundCommand { get; }
    public event EventHandler? OnClosed;
    public event EventHandler<IList<ImportSettings>>? OnConfirmed;

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

    public record ScalingOption(string Name, Func<Size, BitmapScalingMode> Mode, Func<Size, IResampler> Sampler);
    public ScalingOption ScaleChoice
    {
        get { return ScaleOptions[Properties.Settings.Default.ScaleChoice]; }
        set { Properties.Settings.Default.ScaleChoice = ScaleOptions.IndexOf(value); OnPropertyChanged(); OnPropertyChanged(nameof(CurrentMode)); }
    }
    public ReadOnlyCollection<ScalingOption> ScaleOptions { get; } = new List<ScalingOption>
    {
        new ScalingOption("Automatic", x => x.Width > 128 && x.Height > 128 ? BitmapScalingMode.HighQuality : BitmapScalingMode.NearestNeighbor, x => x.Width > 128 && x.Height > 128 ? KnownResamplers.Bicubic : KnownResamplers.NearestNeighbor),
        new ScalingOption("Pixel Art", x => BitmapScalingMode.NearestNeighbor, x => KnownResamplers.NearestNeighbor),
        new ScalingOption("Bicubic", x => BitmapScalingMode.HighQuality, x => KnownResamplers.Bicubic)
    }.AsReadOnly();
    public BitmapScalingMode CurrentMode => CurrentImage == null ? BitmapScalingMode.NearestNeighbor : ScaleChoice.Mode(CurrentImage.Source.Image.Value.Size());

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

    public record BackgroundColorOption(Brush Brush, Rgba32 Pixel);
    public BackgroundColorOption BackgroundColorChoice
    {
        get { return BackgroundColorOptions[Properties.Settings.Default.BackgroundColorChoice]; }
        set { Properties.Settings.Default.BackgroundColorChoice = BackgroundColorOptions.IndexOf(value); OnPropertyChanged(); }
    }
    public ReadOnlyCollection<BackgroundColorOption> BackgroundColorOptions { get; } = new List<BackgroundColorOption>
    {
        new BackgroundColorOption(Brushes.Transparent, SixLabors.ImageSharp.Color.Transparent),
        new BackgroundColorOption(Brushes.White, SixLabors.ImageSharp.Color.White),
        new BackgroundColorOption(Brushes.Black, SixLabors.ImageSharp.Color.Black)
    }.AsReadOnly();

    public ObservableCollection<PreviewImage> ImageQueue { get; } = new();
    private int CurrentIndex = 0;
    public PreviewImage? CurrentImage => ImageQueue.Count == 0 ? null : ImageQueue[CurrentIndex];

    public ImportViewModel()
    {
        RotateCommand = new RelayCommand<float>(val =>
        {
            if (CurrentImage != null)
                CurrentImage.Rotation = (CurrentImage.Rotation + val * Math.Sign(CurrentImage.ScaleX * CurrentImage.ScaleY)) % 360;
        });
        HorizontalFlipCommand = new RelayCommand(() =>
        {
            if (CurrentImage != null)
                CurrentImage.ScaleX *= -1;
        });
        VerticalFlipCommand = new RelayCommand(() =>
        {
            if (CurrentImage != null)
                CurrentImage.ScaleY *= -1;
        });
        SwitchImageCommand = new RelayCommand<PreviewImage>(preview =>
        {
            CurrentIndex = ImageQueue.IndexOf(preview);
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
        });
        DiscardCommand = new RelayCommand(() =>
        {
            ImageQueue.RemoveAt(CurrentIndex);
            if (CurrentIndex >= ImageQueue.Count)
                CurrentIndex--;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
            CloseIfDone();
        });
        DiscardAllCommand = new RelayCommand(() =>
        {
            ImageQueue.Clear();
            CurrentIndex = 0;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
            CloseIfDone();
        });
        ConfirmCommand = new RelayCommand(() =>
        {
            if (CurrentImage != null)
                ConfirmImages(new[] { CurrentImage });
            ImageQueue.RemoveAt(CurrentIndex);
            if (CurrentIndex >= ImageQueue.Count)
                CurrentIndex--;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
            CloseIfDone();
        });
        ConfirmAllCommand = new RelayCommand(() =>
        {
            ConfirmImages(ImageQueue);
            ImageQueue.Clear();
            CurrentIndex = 0;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
            CloseIfDone();
        });
        NavigateCommand = new RelayCommand<int>(x =>
        {
            CurrentIndex = ((CurrentIndex + x) % ImageQueue.Count + ImageQueue.Count) % ImageQueue.Count;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(CurrentMode));
        });
        ChangeBackgroundCommand = new RelayCommand(() =>
        {
            BackgroundColorChoice = BackgroundColorOptions[(BackgroundColorOptions.IndexOf(BackgroundColorChoice) + 1) % BackgroundColorOptions.Count];
        });
    }

    private void ConfirmImages(IEnumerable<PreviewImage> previews)
    {
        var settings = new List<ImportSettings>();
        foreach (var preview in previews)
        {
            settings.Add(new ImportSettings(preview, GridWidth, GridHeight, new(() => ScaleChoice.Sampler(preview.Source.Image.Value.Size())), StretchChoice.Mode, BackgroundColorChoice.Pixel, new ProcessSettings(DitherChoice.Dither, AlgorithmChoice.Algorithm)));
        }
        OnConfirmed?.Invoke(this, settings);
    }

    private void CloseIfDone()
    {
        if (ImageQueue.Count == 0)
            OnClosed?.Invoke(this, EventArgs.Empty);
    }

    public void AddImages(IEnumerable<PendingSource> sources)
    {
        foreach (var source in sources)
        {
            ImageQueue.Add(new PreviewImage(source));
        }
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(CurrentMode));
        HadMultiple = ImageQueue.Count > 1;
    }
}

public class PendingSource
{
    public Lazy<ImageSource> Source { get; }
    public Remakable<Image<Rgba32>> Image { get; }
    public string Name { get; }

    public PendingSource(Lazy<ImageSource> source, Func<Image<Rgba32>> image, string name)
    {
        Source = source;
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
        return new PendingSource(new(() =>
        {
            var img = new BitmapImage(new Uri(path));
            img.Freeze();
            return img;
        }), () => SixLabors.ImageSharp.Image.Load<Rgba32>(path),
        Path.GetFileName(path));
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

public record ImportSettings(PreviewImage Preview, int Width, int Height, Lazy<IResampler> Sampler, ResizeMode ResizeMode, Rgba32 BackgroundColor, ProcessSettings ProcessSettings);

public record ProcessSettings(IDither? Dither, IColorAlgorithm Algorithm);
