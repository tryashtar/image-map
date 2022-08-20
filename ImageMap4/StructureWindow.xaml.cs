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
    public StructureWindow(StructureViewModel context)
    {
        InitializeComponent();
        this.DataContext = context;
        ViewModel.OnClosed += (s, e) => this.Close();
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
