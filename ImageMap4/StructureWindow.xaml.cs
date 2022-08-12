using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for StructureWindow.xaml
/// </summary>
public partial class StructureWindow : Window
{
    public StructureViewModel ViewModel => (StructureViewModel)DataContext;
    public StructureWindow()
    {
        InitializeComponent();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StructureViewModel.Grid))
            UpdateGrid();
    }

    private void UpdateGrid()
    {
        StructureGrid.SplitGrid.Children.Clear();
        var selected = ViewModel.Parent.ExistingMaps.Where(x => x.IsSelected).ToList();
        for (int i = 0; i < Math.Min(selected.Count, ViewModel.GridWidth * ViewModel.GridHeight); i++)
        {
            var item = new Image { Source = selected[i].Item.Data.ImageSource };
            Grid.SetColumn(item, i % ViewModel.GridWidth);
            Grid.SetRow(item, i / ViewModel.GridWidth);
            StructureGrid.SplitGrid.Children.Add(item);
        }
    }
}
