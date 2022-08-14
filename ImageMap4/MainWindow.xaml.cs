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
using System.Windows.Shapes;
using System.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;

namespace ImageMap4;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDropTarget
{
    private MainViewModel ViewModel => (MainViewModel)this.DataContext;
    public ICommand PasteCommand { get; }
    public ICommand OpenWorldFolderCommand { get; }
    public MainWindow()
    {
        InitializeComponent();
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

    private ImportWindow? ImportWindow;
    private void OpenImages(IEnumerable<PendingSource> images)
    {
        if (ImportWindow == null || !ImportWindow.IsVisible)
        {
            ImportWindow = new(ViewModel.SelectedWorld is JavaWorld);
            ImportWindow.Owner = this;
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
            StructureWindow = new();
            StructureWindow.Owner = this;
            StructureWindow.ViewModel.Parent = this.ViewModel;
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
        if (info.VisualTarget == ImportList && info.Data is IDataObject data && data.GetDataPresent(DataFormats.FileDrop))
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
        return () => { };
    }
}
