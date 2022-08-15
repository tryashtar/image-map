using GongSolutions.Wpf.DragDrop;
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
public partial class StructureWindow : Window, IDropTarget
{
    public StructureViewModel ViewModel => (StructureViewModel)DataContext;
    public StructureWindow()
    {
        InitializeComponent();
        UpdateGrid();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        ViewModel.OnClosed += (s, e) => this.Close();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StructureViewModel.Grid))
            UpdateGrid();
    }

    private void UpdateGrid()
    {
        StructureGrid.SplitGrid.Children.Clear();
        for (int y = 0; y < ViewModel.GridHeight; y++)
        {
            for (int x = 0; x < ViewModel.GridWidth; x++)
            {
                var preview = new MapPreview() { DataContext = ViewModel.Grid[x, y]?.Data };
                Grid.SetColumn(preview, x);
                Grid.SetRow(preview, y);
                GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(preview, true);
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(preview, this);
                GongSolutions.Wpf.DragDrop.DragDrop.SetIsDragSource(preview, true);
                GongSolutions.Wpf.DragDrop.DragDrop.SetUseDefaultDragAdorner(preview, true);
                StructureGrid.SplitGrid.Children.Add(preview);
            }
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
        info.Effects = DragDropEffects.Move;
        return () =>
        {
            int from_x = Grid.GetColumn(info.DragInfo.VisualSource);
            int from_y = Grid.GetRow(info.DragInfo.VisualSource);
            int to_x = Grid.GetColumn(info.VisualTargetItem);
            int to_y = Grid.GetRow(info.VisualTargetItem);
            ViewModel.MoveMap(from_x, from_y, to_x, to_y);
        };
    }
}
