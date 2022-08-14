using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using SixLabors.ImageSharp.Processing;
using System.Globalization;
using System.Windows.Controls;

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
        UpdateGrid();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.CurrentImage))
        {
            var item = NavigationBar.ItemContainerGenerator.ContainerFromItem(ViewModel.CurrentImage);
            if (item is FrameworkElement el)
                el.BringIntoView();
        }
        else if (e.PropertyName == nameof(ImportViewModel.GridHeight) || e.PropertyName == nameof(ImportViewModel.GridWidth))
            UpdateGrid();
    }

    private void UpdateGrid()
    {
        PreviewGrid.SplitGrid.Children.Clear();
        for (int y = 0; y < ViewModel.GridHeight; y++)
        {
            for (int x = 0; x < ViewModel.GridWidth; x++)
            {
                var preview = new Image() { Source = (ImageSource)this.Resources["MapBackground"] };
                RenderOptions.SetBitmapScalingMode(preview, BitmapScalingMode.NearestNeighbor);
                Grid.SetColumn(preview, x);
                Grid.SetRow(preview, y);
                PreviewGrid.SplitGrid.Children.Add(preview);
            }
        }
    }
}
