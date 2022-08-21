using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SixLabors.ImageSharp;

namespace ImageMap4;
/// <summary>
/// Interaction logic for ImageWindow.xaml
/// </summary>
public partial class ImageWindow : Window
{
    public ImageViewModel ViewModel => (ImageViewModel)DataContext;
    public ICommand CancelCommand { get; }
    public ICommand ExportCombinedCommand { get; }
    public ICommand ExportSeparateCommand { get; }
    public ImageWindow(ImageViewModel context)
    {
        this.DataContext = context;
        CancelCommand = new RelayCommand(() => this.Close());
        ExportCombinedCommand = new RelayCommand(() =>
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Export the full image";
            dialog.Filter = "Image Files|*.png;|All Files|*";
            if (dialog.ShowDialog() == true)
            {
                using var image = ViewModel.CreateImage();
                image.Save(dialog.FileName);
                this.Close();
            }
        });
        ExportSeparateCommand = new RelayCommand(() =>
        {
            var dialog = new VistaFolderBrowserDialog();
            var maps = ViewModel.GridMaker.MapSource.Where(x => x.IsSelected).ToList();
            dialog.Description = $"Select the folder to save {maps.Count} maps to";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog() == true)
            {
                foreach (var item in maps)
                {
                    item.Item.Data.Image.Save(Path.Combine(dialog.SelectedPath, $"map_{item.Item.ID}.png"));
                }
                this.Close();
            }
        });
        InitializeComponent();
        // a bit of a hack
        // I wanted to make the frame backgrounds not show up
        GridMaker.StructureGrid.CellContentsTemplate = (DataTemplate)this.Resources["Template"];
    }
}
