using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using SixLabors.ImageSharp.Processing;
using System.Globalization;

namespace ImageMap4;
/// <summary>
/// Interaction logic for ImportWindow.xaml
/// </summary>
public partial class ImportWindow : Window
{
    private ImportViewModel ViewModel => (ImportViewModel)DataContext;
    public ImportWindow(IEnumerable<string> images, bool java)
    {
        InitializeComponent();
        ViewModel.JavaMode = java;
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
        // ensures cells of grid are perfect squares
        var grid_ratio = (double)ViewModel.GridWidth / ViewModel.GridHeight;
        var space_ratio = SpaceGrid.ActualWidth / Math.Max(1, SpaceGrid.ActualHeight); // avoid divide by zero
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

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Properties.Settings.Default.StretchChoice = ViewModel.StretchOptions.IndexOf(ViewModel.StretchChoice);
        Properties.Settings.Default.ScaleChoice = ViewModel.ScaleOptions.IndexOf(ViewModel.ScaleChoice);
    }
}
