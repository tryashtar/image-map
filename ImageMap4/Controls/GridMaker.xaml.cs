using GongSolutions.Wpf.DragDrop;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageMap4;
/// <summary>
/// Interaction logic for GridMaker.xaml
/// </summary>
public partial class GridMaker : UserControl, IDropTarget
{
    public static readonly DependencyProperty ControlsProperty =
             DependencyProperty.Register(nameof(Controls), typeof(FrameworkElement),
             typeof(GridMaker), new FrameworkPropertyMetadata());
    public FrameworkElement Controls
    {
        get { return (FrameworkElement)GetValue(ControlsProperty); }
        set { SetValue(ControlsProperty, value); }
    }
    public GridMakerViewModel ViewModel => (GridMakerViewModel)DataContext;
    public GridMaker()
    {
        InitializeComponent();
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
