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
        const double thickness = 3;
        Pen black = new Pen(Brushes.Black, thickness);
        Pen white = new Pen(Brushes.White, 1);
        black.Freeze();
        white.Freeze();

        for (int i = 0; i < this.Rows + 1; i++)
        {
            double y = (this.ActualHeight - thickness) * i / this.Rows + thickness / 2;
            dc.DrawLine(black, new Point(0, y), new Point(this.ActualWidth, y));
            dc.DrawLine(white, new Point(0, y), new Point(this.ActualWidth, y));
        }

        for (int i = 0; i < this.Columns + 1; i++)
        {
            double x = (this.ActualWidth - thickness) * i / this.Columns + thickness / 2;
            dc.DrawLine(black, new Point(x, 0), new Point(x, this.ActualHeight));
            dc.DrawLine(white, new Point(x, 0), new Point(x, this.ActualHeight));
        }

        base.OnRender(dc);
    }
}
