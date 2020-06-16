using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    // a picture box that supports drawing with a certain interpolation
    public class InterpPictureBox : PictureBox
    {
        private InterpolationMode InterpPrivate;
        public InterpolationMode Interp
        {
            get => InterpPrivate;
            set { InterpPrivate = value; this.Refresh(); }
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = Interp;
            base.OnPaint(paintEventArgs);
        }
    }

    public class MapPreviewBox : PictureBox
    {
        public readonly Map Map;
        private readonly Image OriginalImage;

        public MapPreviewBox(Map map)
        {
            Map = map;
            OriginalImage = map.Original;
            Constructor();
        }

        private void Constructor()
        {
            Width = 128;
            Height = 128;
            SizeMode = PictureBoxSizeMode.Zoom;
            Image = Map.Image;
            BackgroundImage = Properties.Resources.item_frame;
            MouseEnter += MapPreviewBox_MouseEnter;
            MouseLeave += MapPreviewBox_MouseLeave;
        }

        private void MapPreviewBox_MouseLeave(object sender, EventArgs e)
        {
            Image = Map.Image;
        }

        private void MapPreviewBox_MouseEnter(object sender, EventArgs e)
        {
            Image = OriginalImage;
        }
    }
}
