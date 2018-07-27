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
        public DualEditionMap Maps { get; private set; }
        public Edition ViewingEdition { get; private set; }

        public MapPreviewBox(Bitmap original, Edition start)
        {
            Maps = new DualEditionMap(original);
            Image = original;
            BackgroundImage = Properties.Resources.item_frame;
            MouseEnter += MapPreviewBox_MouseEnter;
            MouseLeave += MapPreviewBox_MouseLeave;
            ViewEdition(start);
        }

        public void ViewEdition(Edition view)
        {
            ViewingEdition = view;
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                Image = Maps.OriginalImage;
            else
            {
                if (ViewingEdition == Edition.Java)
                    Image = Maps.GetJavaMap().Image;
                else if (ViewingEdition == Edition.Bedrock)
                    Image = Maps.GetBedrockMap().Image;
            }
        }

        private void MapPreviewBox_MouseLeave(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void MapPreviewBox_MouseEnter(object sender, EventArgs e)
        {
            UpdateImage();
        }
    }
}
