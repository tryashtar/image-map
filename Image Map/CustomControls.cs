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
        public Bitmap OriginalImage;
        public Bitmap JavaImage { get; private set; }
        public Bitmap BedrockImage { get; private set; }
        private BackgroundWorker ImageSetter = new BackgroundWorker();
        private bool viewingjava;
        public bool ViewingJava
        {
            get => viewingjava;
            set
            {
                viewingjava = value;
                if ((viewingjava && JavaImage == null) || (!viewingjava && BedrockImage == null))
                    ImageSetter.RunWorkerAsync();
                UpdateMainImage();
            }
        }

        public MapPreviewBox(Bitmap original)
        {
            OriginalImage = original;
            Image = original;
            MouseEnter += MapPreviewBox_MouseEnter;
            MouseLeave += MapPreviewBox_MouseLeave;
            ImageSetter.DoWork += ImageSetter_DoWork;
            ImageSetter.RunWorkerCompleted += ImageSetter_RunWorkerCompleted;
        }

        private void UpdateMainImage()
        {
            if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                MapPreviewBox_MouseEnter(null, null);
            else
                MapPreviewBox_MouseLeave(null, null);
        }

        private void ImageSetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateMainImage();
        }

        private void ImageSetter_DoWork(object sender, DoWorkEventArgs e)
        {
            if (viewingjava)
                JavaImage = MapHelpers.JavaMapify(OriginalImage);
            else
                BedrockImage = MapHelpers.BedrockMapify(OriginalImage);
        }

        private void MapPreviewBox_MouseLeave(object sender, EventArgs e)
        {
            if (viewingjava)
                Image = JavaImage;
            else
                Image = BedrockImage;
        }

        private void MapPreviewBox_MouseEnter(object sender, EventArgs e)
        {
            Image = OriginalImage;
        }
    }
}
