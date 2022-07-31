using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageMap4;
public class SquareGrid : UniformGrid
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
