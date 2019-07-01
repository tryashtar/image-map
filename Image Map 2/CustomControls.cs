using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
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
        public Map Map { get; private set; }
        private Image OriginalImage;

        public MapPreviewBox(PreMap original, Edition edition)
        {
            // provide an original image, and generate and remember the converted version
            if (edition == Edition.Java)
                Map = new JavaMap(original.Contents, original.Dithered);
            else if (edition == Edition.Bedrock)
                Map = new BedrockMap(original.Contents);
            OriginalImage = original.Contents;
            Constructor();
        }

        public MapPreviewBox(Map map)
        {
            // no original image is provided, so the two images are the same
            Map = map;
            OriginalImage = map.Image.GetImage();
            Constructor();
        }

        private void Constructor()
        {
            Width = 128;
            Height = 128;
            SizeMode = PictureBoxSizeMode.Zoom;
            Image = Map.Image.GetImage();
            BackgroundImage = Properties.Resources.item_frame;
            MouseEnter += MapPreviewBox_MouseEnter;
            MouseLeave += MapPreviewBox_MouseLeave;
        }

        private void MapPreviewBox_MouseLeave(object sender, EventArgs e)
        {
            Image = Map.Image.GetImage();
        }

        private void MapPreviewBox_MouseEnter(object sender, EventArgs e)
        {
            Image = OriginalImage;
        }
    }
}
