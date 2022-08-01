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
    public ImportViewModel ViewModel => (ImportViewModel)DataContext;
    public ImportWindow(bool java)
    {
        InitializeComponent();
        ViewModel.JavaMode = java;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        ViewModel.OnClosed += (s, e) => this.Close();
        SpaceGrid.SizeChanged += SpaceGrid_SizeChanged;
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
        if (e.PropertyName == nameof(ImportViewModel.CurrentImage))
        {
            var item = NavigationBar.ItemContainerGenerator.ContainerFromItem(ViewModel.CurrentImage);
            if (item is FrameworkElement el)
                el.BringIntoView();
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
}
