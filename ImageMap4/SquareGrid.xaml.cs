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
    public FrameworkElement Inside
    {
        get { return (FrameworkElement)GetValue(InsideProperty); }
        set { SetValue(InsideProperty, value); }
    }

    public SquareGrid()
    {
        InitializeComponent();
        SpaceGrid.SizeChanged += SpaceGrid_SizeChanged;
    }

    private static void DimensionsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((SquareGrid)sender).FixSpace();
    }

    private void SpaceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSpace();
    }

    private void FixSpace()
    {
        // ensures cells of grid are perfect squares
        var grid_ratio = (double)Columns / Rows;
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

internal class InternalSquareGrid : UniformGrid
{
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
