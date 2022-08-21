using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for SquareGrid.xaml
/// </summary>
public partial class SquareGrid : UserControl
{
    public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int),
            typeof(SquareGrid), new FrameworkPropertyMetadata(1, DimensionsChanged));
    public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int),
            typeof(SquareGrid), new FrameworkPropertyMetadata(1, DimensionsChanged));
    public static readonly DependencyProperty InsideProperty =
             DependencyProperty.Register(nameof(Inside), typeof(FrameworkElement),
             typeof(SquareGrid), new FrameworkPropertyMetadata());
    public static readonly DependencyProperty CellContentsTemplateProperty =
             DependencyProperty.Register(nameof(CellContentsTemplate), typeof(DataTemplate),
             typeof(SquareGrid), new FrameworkPropertyMetadata(ContentsChanged));
    public static readonly DependencyProperty CellContentsContextProperty =
             DependencyProperty.Register(nameof(CellContentsContext), typeof(object[,]),
             typeof(SquareGrid), new FrameworkPropertyMetadata(ContextChanged));
    public int Columns
    {
        get { return (int)GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }
    public int Rows
    {
        get { return (int)GetValue(RowsProperty); }
        set { SetValue(RowsProperty, value); }
    }

    // goes on top of the grid, but underneath the gridlines
    public FrameworkElement Inside
    {
        get { return (FrameworkElement)GetValue(InsideProperty); }
        set { SetValue(InsideProperty, value); }
    }

    // goes in each cell of the grid
    public DataTemplate CellContentsTemplate
    {
        get { return (DataTemplate)GetValue(CellContentsTemplateProperty); }
        set { SetValue(CellContentsTemplateProperty, value); }
    }
    public object[,] CellContentsContext
    {
        get { return (object[,])GetValue(CellContentsContextProperty); }
        set { SetValue(CellContentsContextProperty, value); }
    }

    public SquareGrid()
    {
        InitializeComponent();
        SpaceGrid.SizeChanged += SpaceGrid_SizeChanged;
    }

    private static void DimensionsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((SquareGrid)sender).FixSpace();
        ((SquareGrid)sender).ReshapeCells();
    }

    private static void ContentsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((SquareGrid)sender).ReplaceCells();
    }

    private static void ContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((SquareGrid)sender).RelinkCells();
    }

    private void SpaceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSpace();
    }

    private void FixSpace()
    {
        // ensures cells of grid are perfect squares
        var grid_ratio = (double)Columns / Rows;
        var space_ratio = Math.Max(1, SpaceGrid.ActualWidth) / Math.Max(1, SpaceGrid.ActualHeight); // avoid divide by zero
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

    private FrameworkElement[,] Cells;
    private void ReplaceCells()
    {
        SplitGrid.Children.Clear();
        if (CellContentsTemplate == null)
            return;
        if (Cells == null)
            Cells = new FrameworkElement[Columns, Rows];
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                Cells[x, y] = AddItem(x, y);
            }
        }
    }
    private FrameworkElement AddItem(int x, int y)
    {
        var item = (FrameworkElement)CellContentsTemplate.LoadContent();
        if (CellContentsContext != null)
            item.DataContext = CellContentsContext[x, y];
        SplitGrid.Children.Insert(Math.Min(SplitGrid.Children.Count, y * Columns + x), item);
        // doesn't actually do anything to this control, but lets you query it
        Grid.SetColumn(item, x);
        Grid.SetRow(item, y);
        return item;
    }
    private void ReshapeCells()
    {
        SplitGrid.Rows = this.Rows;
        SplitGrid.Columns = this.Columns;
        var newcells = new FrameworkElement[Columns, Rows];
        for (int y = 0; y < Math.Max(Rows, Cells.GetLength(1)); y++)
        {
            for (int x = 0; x < Math.Max(Columns, Cells.GetLength(0)); x++)
            {
                if (x >= Columns || y >= Rows)
                    SplitGrid.Children.Remove(Cells[x, y]);
                else if (x >= Cells.GetLength(0) || y >= Cells.GetLength(1))
                    newcells[x, y] = AddItem(x, y);
                else
                    newcells[x, y] = Cells[x, y];
            }
        }
        Cells = newcells;
    }
    private void RelinkCells()
    {
        for (int y = 0; y < Math.Min(Rows, CellContentsContext.GetLength(1)); y++)
        {
            for (int x = 0; x < Math.Min(Columns, CellContentsContext.GetLength(0)); x++)
            {
                Cells[x, y].DataContext = CellContentsContext[x, y];
            }
        }
    }
}

internal class GridlineRenderer : FrameworkElement
{
    public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int),
            typeof(GridlineRenderer), new FrameworkPropertyMetadata(1, RatioChanged));
    public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int),
            typeof(GridlineRenderer), new FrameworkPropertyMetadata(1, RatioChanged));
    public int Columns
    {
        get { return (int)GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }
    public int Rows
    {
        get { return (int)GetValue(RowsProperty); }
        set { SetValue(RowsProperty, value); }
    }

    // fixes grid lines not updating if both grid and columns change without ratio changing, e.g. 1x1 -> 2x2
    private static void RatioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((GridlineRenderer)sender).InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        Pen black = new Pen(Brushes.Black, 4);
        Pen white = new Pen(Brushes.White, 1);
        black.Freeze();
        white.Freeze();

        // thick black lines
        for (int i = 0; i < this.Rows + 1; i++)
        {
            double y = (this.ActualHeight - black.Thickness) * i / this.Rows + black.Thickness / 2;
            dc.DrawLine(black, new Point(0, y), new Point(this.ActualWidth, y));
        }
        for (int i = 0; i < this.Columns + 1; i++)
        {
            double x = (this.ActualWidth - black.Thickness) * i / this.Columns + black.Thickness / 2;
            dc.DrawLine(black, new Point(x, 0), new Point(x, this.ActualHeight));
        }

        // inner white lines
        for (int i = 0; i < this.Rows + 1; i++)
        {
            double y = (this.ActualHeight - black.Thickness) * i / this.Rows + black.Thickness / 2;
            dc.DrawLine(white, new Point(black.Thickness / 2, y), new Point(this.ActualWidth - black.Thickness / 2, y));
        }
        for (int i = 0; i < this.Columns + 1; i++)
        {
            double x = (this.ActualWidth - black.Thickness) * i / this.Columns + black.Thickness / 2;
            dc.DrawLine(white, new Point(x, black.Thickness / 2), new Point(x, this.ActualHeight - black.Thickness / 2));
        }

        base.OnRender(dc);
    }
}
