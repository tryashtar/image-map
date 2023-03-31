using System;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Visuals.Media.Imaging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using IImage = Avalonia.Media.IImage;
using Size = Avalonia.Size;

namespace ImageMap4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void JavaWorldsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.JavaFolders ??= new();
        bool adding = false;
        var dialog = new OpenFolderDialog();
        dialog.Title = adding
            ? "Add a Java world folder to the list"
            : "Select the folder where your Java worlds are saved";
        if (Properties.Settings.Default.JavaFolders.Count > 0)
            dialog.Directory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolders[0]);
        var path = await dialog.ShowAsync(this);
        if (path != null)
        {
            if (!adding)
                Properties.Settings.Default.JavaFolders.Clear();
            Properties.Settings.Default.JavaFolders.Add(path);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private async void BedrockWorldsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.BedrockFolders ??= new();
        bool adding = false;
        var dialog = new OpenFolderDialog();
        dialog.Title = adding
            ? "Add a Bedrock world folder to the list"
            : "Select the folder where your Bedrock worlds are saved";
        if (Properties.Settings.Default.BedrockFolders.Count > 0)
            dialog.Directory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolders[0]);
        var path = await dialog.ShowAsync(this);
        if (path != null)
        {
            if (!adding)
                Properties.Settings.Default.BedrockFolders.Clear();
            Properties.Settings.Default.BedrockFolders.Add(path);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }
}

public class ImageSharpConverter : OneWayConverter<Image<Rgba32>, IImage>
{
    private static readonly ConditionalWeakTable<Image<Rgba32>, IImage> Cache = new();

    public override IImage Convert(Image<Rgba32> value)
    {
        return Cache.GetValue(value, x =>
        {
            using var ms = new MemoryStream();
            value.Save(ms, PngFormat.Instance);
            ms.Position = 0;
            return new Avalonia.Media.Imaging.Bitmap(ms);
        });
    }
}
