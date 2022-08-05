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
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.CurrentImage))
        {
            var item = NavigationBar.ItemContainerGenerator.ContainerFromItem(ViewModel.CurrentImage);
            if (item is FrameworkElement el)
                el.BringIntoView();
        }
    }
}
