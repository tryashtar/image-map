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

namespace ImageMap4;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDropTarget
{
    private MainViewModel ViewModel => (MainViewModel)this.DataContext;
    public ICommand PasteCommand { get; }
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
                var image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
                OpenImages(new[]
                {
                    new PendingSource(new(source), new(image), "Pasted image")
                });
            }
        });
        OpenWorldFolderCommand = new RelayCommand<World>(x =>
        {
            Process.Start("explorer.exe", x.Folder);
        });
        OpenMapFileCommand = new RelayCommand<Map>(x =>
        {
            Process.Start("explorer.exe", $"/select, \"{Path.Combine(ViewModel.SelectedWorld.Folder, "data", $"map_{x.ID}.dat")}\"");
        });
        ChangeIDCommand = new RelayCommand<IList<Selectable<Map>>>(x =>
        {
            var selected = x.Where(x => x.IsSelected);
            var window = new ChangeIDWindow(selected.FirstOrDefault()?.Item.ID ?? 0);
            window.Owner = this;
            if (window.ShowDialog() ?? false)
            {
                if (window.Result == ChangeResult.Confirmed)
                    ViewModel.ChangeIDs(selected, window.ID);
                else if (window.Result == ChangeResult.Auto)
                    ViewModel.AutoIDs(selected);
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
        InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Properties.Settings.Default.Save();
    }

    private void JavaFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = "Select the folder where your Java worlds are saved";
        dialog.UseDescriptionForTitle = true;
        dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolder);
        if (dialog.ShowDialog() == true)
        {
            Properties.Settings.Default.JavaFolder = dialog.SelectedPath;
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private void BedrockFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = "Select the folder where your Bedrock worlds are saved";
        dialog.UseDescriptionForTitle = true;
        dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolder);
        if (dialog.ShowDialog() == true)
        {
            Properties.Settings.Default.BedrockFolder = dialog.SelectedPath;
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
            ImportWindow.ViewModel.OnConfirmed += (s, e) => ViewModel.AddImport(e);
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
            StructureWindow.ViewModel.OnConfirmed += (s, e) => ViewModel.SelectedWorld.AddStructure(e.grid, e.inventory);
        }
        StructureWindow.Show();
        StructureWindow.Activate();
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
                    if (Directory.Exists(file) && File.Exists(Path.Combine(file, "level.dat")))
                    {
                        if (Directory.Exists(Path.Combine(file, "db")))
                            ViewModel.SelectedWorld = new BedrockWorld(file);
                        else
                            ViewModel.SelectedWorld = new JavaWorld(file);
                        TabList.SelectedItem = MapsTab;
                    }
                };
            }
        }
        return () => { };
    }

    private void JavaWorldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (JavaWorldList.SelectedIndex != -1)
        {
            BedrockWorldList.SelectedIndex = -1;
            ViewModel.SelectedWorld = (World)JavaWorldList.SelectedItem;
            TabList.SelectedItem = MapsTab;
        }
    }

    private void BedrockWorldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BedrockWorldList.SelectedIndex != -1)
        {
            JavaWorldList.SelectedIndex = -1;
            ViewModel.SelectedWorld = (World)BedrockWorldList.SelectedItem;
            TabList.SelectedItem = MapsTab;
        }
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
