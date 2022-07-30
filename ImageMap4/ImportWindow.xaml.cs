using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Globalization;

namespace ImageMap4;
/// <summary>
/// Interaction logic for ImportWindow.xaml
/// </summary>
public partial class ImportWindow : Window
{
    private ImportViewModel ViewModel => (ImportViewModel)DataContext;
    private Stretch ImageStretch => ViewModel.FillFrames ? Stretch.Fill : Stretch.Uniform;
    public ImportWindow(IEnumerable<string> images)
    {
        InitializeComponent();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        SpaceGrid.SizeChanged += SpaceGrid_SizeChanged;
        ViewModel.AddImages(images);
    }

    private void SpaceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSpace();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.GridWidth) || e.PropertyName == nameof(ImportViewModel.GridHeight))
        {
            SplitGrid.InvalidateVisual();
            FixSpace();
        }
    }

    private void FixSpace()
    {
        var grid_ratio = (double)ViewModel.GridWidth / ViewModel.GridHeight;
        var space_ratio = SpaceGrid.ActualWidth / SpaceGrid.ActualHeight;
        if (space_ratio > grid_ratio)
        {
            SpaceGrid.RowDefinitions[0].Height = new GridLength(0);
            SpaceGrid.RowDefinitions[2].Height = new GridLength(0);
            SpaceGrid.ColumnDefinitions[0].Width = new GridLength(0.5 * space_ratio / grid_ratio - 0.5, GridUnitType.Star);
            SpaceGrid.ColumnDefinitions[2].Width = new GridLength(0.5 * space_ratio / grid_ratio - 0.5, GridUnitType.Star);
        }
        else if (space_ratio < grid_ratio)
        {
            SpaceGrid.ColumnDefinitions[0].Width = new GridLength(0);
            SpaceGrid.ColumnDefinitions[2].Width = new GridLength(0);
            SpaceGrid.RowDefinitions[0].Height = new GridLength(0.5 * grid_ratio / space_ratio - 0.5, GridUnitType.Star);
            SpaceGrid.RowDefinitions[2].Height = new GridLength(0.5 * grid_ratio / space_ratio - 0.5, GridUnitType.Star);
        }
    }

    public List<Map> Maps;

    private void RotateButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CurrentImage.Mutate(x => x.Rotate(90));
        ViewModel.OnPropertyChanged(nameof(ViewModel.CurrentSource));
    }
}

public class StretchConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Stretch.Fill : Stretch.Uniform;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}