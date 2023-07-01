using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Visuals.Media.Imaging;
using fNbt;
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

    public MainViewModel ViewModel => (MainViewModel)DataContext;

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

    private void JavaWorldList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (JavaWorldList.SelectedIndex != -1)
        {
            BedrockWorldList.SelectedIndex = -1;
            TryOpenWorld((IWorld)JavaWorldList.SelectedItem);
        }
    }

    private void BedrockWorldList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (BedrockWorldList.SelectedIndex != -1)
        {
            JavaWorldList.SelectedIndex = -1;
            TryOpenWorld((IWorld)BedrockWorldList.SelectedItem);
        }
    }

    private void TryOpenWorld(IWorld world)
    {
        try
        {
            ViewModel.SelectedWorld = world;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}

public class TitleConverter : OneWayConverter<IWorld, string>
{
    public override string Convert(IWorld value)
    {
        if (value == null)
            return "Image Map";
        return $"{value.Name} – Image Map";
    }
}

public class FileNameConverter : OneWayConverter<string, string>
{
    public override string Convert(string value)
    {
        return Path.GetFileName(value);
    }
}

public class ImageSharpConverter : OneWayConverter<Image<Rgba32>, IImage>
{
    private static readonly ConditionalWeakTable<Image<Rgba32>, IImage> Cache = new();

    public override IImage Convert(Image<Rgba32> value)
    {
        if (value == null)
            return null;
        return Cache.GetValue(value, x =>
        {
            using var ms = new MemoryStream();
            value.Save(ms, PngFormat.Instance);
            ms.Position = 0;
            return new Avalonia.Media.Imaging.Bitmap(ms);
        });
    }
}

public class DisplayJavaInventory : IInventory, INotifyPropertyChanged
{
    private readonly IInventory Wrapped;
    private static readonly HttpClient Client = new();
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Name { get; private set; }

    public DisplayJavaInventory(IInventory wrapped)
    {
        Name = wrapped.Name;
        if (Name.Length == 36)
        {
            // convert UUIDs to playernames
            // first check in cache, which is like a slow dictionary where keys are in even places and values in odd
            bool found = false;
            if (Properties.Settings.Default.UsernameCache == null)
                Properties.Settings.Default.UsernameCache = new();
            for (int i = 0; i < Properties.Settings.Default.UsernameCache.Count; i += 2)
            {
                if (Properties.Settings.Default.UsernameCache[i] == Name)
                {
                    Name = Properties.Settings.Default.UsernameCache[i + 1];
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // get name from Mojang's API
                // if it fails, just ignore
                // if it succeeds, cache it forever
                Debug.WriteLine($"Looking up UUID {Name}");
                var result = Client.GetAsync($"https://api.mojang.com/user/profiles/{Name}/names");
                result.ContinueWith(x =>
                {
                    if (x.IsCompletedSuccessfully)
                    {
                        var response = x.Result.Content.ReadAsStringAsync().Result;
                        var json = JsonDocument.Parse(response);
                        if (json.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            string newname = json.RootElement[json.RootElement.GetArrayLength() - 1].GetProperty("name")
                                .GetString();
                            lock (Properties.Settings.Default.UsernameCache)
                            {
                                Properties.Settings.Default.UsernameCache.Add(Name);
                                Properties.Settings.Default.UsernameCache.Add(newname);
                            }

                            this.Name = newname;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                        }
                    }
                });
            }
        }
    }

    public void AddItems(IEnumerable<NbtCompound> items)
    {
        Wrapped.AddItems(items);
    }
}