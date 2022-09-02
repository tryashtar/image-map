using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using fNbt;
using System.Net.Http;
using System.Text.Json;

namespace ImageMap4;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDropTarget
{
    private MainViewModel ViewModel => (MainViewModel)this.DataContext;
    public ICommand PasteCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenWorldFolderCommand { get; }
    public ICommand OpenMapFileCommand { get; }
    public ICommand ChangeIDCommand { get; }
    public ICommand ExportImageCommand { get; }
    public MainWindow()
    {
        PasteCommand = new RelayCommand(() =>
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                OpenImages(files.Cast<string>().Select(PendingSource.FromPath));
            }
            else if (Clipboard.ContainsImage())
            {
                var source = Clipboard.GetImage();
                using var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
                stream.Position = 0;
                OpenImages(new[]
                {
                    new PendingSource(() => SixLabors.ImageSharp.Image.Load<Rgba32>(stream), "Pasted image")
                });
            }
        });
        OpenWorldFolderCommand = new RelayCommand<World>(x =>
        {
            Process.Start("explorer.exe", $"\"{x.Folder}\"");
        });
        OpenMapFileCommand = new RelayCommand<Map>(x =>
        {
            Process.Start("explorer.exe", $"/select, \"{Path.Combine(ViewModel.SelectedWorld.Folder, "data", $"map_{x.ID}.dat")}\"");
        });
        ChangeIDCommand = new RelayCommand<IList<Selectable<Map>>>(x =>
        {
            var selected = x.Where(x => x.IsSelected);
            var window = new ChangeIDWindow(selected.FirstOrDefault()?.Item.ID ?? 0, selected.Count(), x.Except(selected).Select(x => x.Item.ID).ToHashSet());
            window.Owner = this;
            if (window.ShowDialog() ?? false)
            {
                if (window.Result == ChangeResult.Confirmed)
                    ViewModel.ChangeIDs(x, selected, window.ID);
                else if (window.Result == ChangeResult.Auto)
                    ViewModel.AutoIDs(x, selected);
            }
        });
        ExportImageCommand = new RelayCommand<ObservableList<Selectable<Map>>>(x =>
        {
            var selected = x.Where(x => x.IsSelected).ToList();
            if (selected.Count == 1)
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "Export the image";
                dialog.Filter = "Image Files|*.png;|All Files|*";
                dialog.FileName = $"map_{selected[0].Item.ID}.png";
                if (dialog.ShowDialog() == true)
                    selected[0].Item.Data.Image.Save(dialog.FileName);
            }
            else
            {
                if (ImageWindow == null || !ImageWindow.IsVisible)
                {
                    ImageWindow = new(new ImageViewModel(new GridMakerViewModel(ViewModel, x)));
                    ImageWindow.Owner = this;
                }
                ImageWindow.Show();
                ImageWindow.Activate();
            }
        });
        DeleteCommand = new RelayCommand(() =>
        {
            if (ImportList.IsFocused)
                ViewModel.DiscardCommand.Execute(ViewModel.ImportingMaps);
            else if (ExistingList.IsFocused)
                ViewModel.DeleteCommand.Execute(ViewModel.ExistingMaps);
        });
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            if (TryOpenWorldFolder(args[1]))
                TabList.SelectedItem = MapsTab;
            if (args.Length > 2)
                OpenImages(args.Skip(2).Select(PendingSource.FromPath));
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Properties.Settings.Default.Save();
    }

    private void JavaFolder_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.JavaFolders ??= new();
        bool adding = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = adding ? "Add a Java world folder to the list" : "Select the folder where your Java worlds are saved";
        dialog.UseDescriptionForTitle = true;
        if (Properties.Settings.Default.JavaFolders.Count > 0)
            dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolders[0]);
        if (dialog.ShowDialog() == true)
        {
            if (!adding)
                Properties.Settings.Default.JavaFolders.Clear();
            Properties.Settings.Default.JavaFolders.Add(dialog.SelectedPath);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private void BedrockFolder_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.BedrockFolders ??= new();
        bool adding = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = adding ? "Add a Bedrock world folder to the list" : "Select the folder where your Bedrock worlds are saved";
        dialog.UseDescriptionForTitle = true;
        if (Properties.Settings.Default.BedrockFolders.Count > 0)
            dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolders[0]);
        if (dialog.ShowDialog() == true)
        {
            if (!adding)
                Properties.Settings.Default.BedrockFolders.Clear();
            Properties.Settings.Default.BedrockFolders.Add(dialog.SelectedPath);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Multiselect = true;
        dialog.Title = "Select images to import";
        dialog.Filter = "Image Files|*.png; *.bmp; *.jpg; *.jpeg; *.gif|All Files|*";
        if (dialog.ShowDialog() == true)
            OpenImages(dialog.FileNames.Select(PendingSource.FromPath));
    }

    private ImageWindow? ImageWindow;
    private ImportWindow? ImportWindow;
    private void OpenImages(IEnumerable<PendingSource> images)
    {
        if (ImportWindow == null || !ImportWindow.IsVisible)
        {
            ImportWindow = new();
            ImportWindow.Owner = this;
            ImportWindow.ViewModel.JavaMode = ViewModel.SelectedWorld is JavaWorld;
            ImportWindow.ViewModel.OnConfirmed += (s, e) => _ = ViewModel.AddImports(e);
        }
        ImportWindow.Show();
        ImportWindow.Activate();
        ImportWindow.ViewModel.AddImages(images);
    }

    private StructureWindow? StructureWindow;
    private void GenerateStructureButton_Click(object sender, RoutedEventArgs e)
    {
        if (StructureWindow == null || !StructureWindow.IsVisible)
        {
            StructureWindow = new(new StructureViewModel(new GridMakerViewModel(this.ViewModel, this.ViewModel.ExistingMaps)));
            StructureWindow.Owner = this;
            StructureWindow.ViewModel.JavaMode = ViewModel.SelectedWorld is JavaWorld;
            StructureWindow.ViewModel.OnConfirmed += (s, e) => Try(() => ViewModel.SelectedWorld.AddStructures(new[] { e.grid }, e.inventory));
        }
        StructureWindow.Show();
        StructureWindow.Activate();
    }

    private void Try(Action action)
    {
        try { action(); }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        GetDropAction(dropInfo);
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        GetDropAction(dropInfo)();
    }

    private Action GetDropAction(IDropInfo info)
    {
        if (info.Data is IDataObject data && data.GetDataPresent(DataFormats.FileDrop))
        {
            if (info.VisualTarget == ImportList)
            {
                info.Effects = DragDropEffects.Copy;
                return () =>
                {
                    var files = (IEnumerable<string>)data.GetData(DataFormats.FileDrop);
                    var list = new List<string>();
                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                            list.Add(file);
                        else if (Directory.Exists(file))
                            list.AddRange(Directory.GetFiles(file));
                    }
                    OpenImages(list.Select(PendingSource.FromPath));
                };
            }
            else if (info.VisualTarget == WorldsTab)
            {
                info.Effects = DragDropEffects.Copy;
                return () =>
                {
                    var file = ((IEnumerable<string>)data.GetData(DataFormats.FileDrop)).First();
                    if (TryOpenWorldFolder(file))
                        TabList.SelectedItem = MapsTab;
                };
            }
        }
        return () => { };
    }

    private bool TryOpenWorldFolder(string folder)
    {
        if (Directory.Exists(folder) && File.Exists(Path.Combine(folder, "level.dat")))
        {
            if (Directory.Exists(Path.Combine(folder, "db")))
                TryOpenWorld(new BedrockWorld(folder));
            else
                TryOpenWorld(new JavaWorld(folder));
            return true;
        }
        return false;
    }

    private void TryOpenWorld(World world)
    {
        try
        {
            ViewModel.SelectedWorld = world;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private void JavaWorldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (JavaWorldList.SelectedIndex != -1)
        {
            BedrockWorldList.SelectedIndex = -1;
            TryOpenWorld((World)JavaWorldList.SelectedItem);
        }
    }

    private void BedrockWorldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BedrockWorldList.SelectedIndex != -1)
        {
            JavaWorldList.SelectedIndex = -1;
            TryOpenWorld((World)BedrockWorldList.SelectedItem);
        }
    }

    private void World_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        TabList.SelectedItem = MapsTab;
    }
}

public class ConflictChecker : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is long id && values[1] is ICollection<long> maps)
            return maps.Contains(id);
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class ImageSharpConverter : OneWayConverter<Image<Rgba32>, ImageSource>
{
    private static readonly ConditionalWeakTable<Image<Rgba32>, ImageSource> Cache = new();

    public override ImageSource Convert(Image<Rgba32> value)
    {
        return Cache.GetValue(value, x =>
        {
            var source = new ImageSharpImageSource<Rgba32>(x);
            source.Freeze();
            return source;
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
                            string newname = json.RootElement[json.RootElement.GetArrayLength() - 1].GetProperty("name").GetString();
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
