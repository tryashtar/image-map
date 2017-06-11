using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public partial class FixImageDialog : Form
    {
        private Image img;
        public FixImageDialog()
        {
            InitializeComponent();
        }
        public FixImageDialog(Image image)
        {
            InitializeComponent();
            img = image;
            ImageBox.Image = image;
            ResultBox.Image = image;
            StretchRadio_CheckedChanged(null, null);
        }

        private void StretchRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (StretchRadio.Checked)
            {
                ResultBox.Image = img;
                ResultBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            if (CropRadio.Checked)
            {
                ResultBox.Image = cropImage(img,new Rectangle(0,0,Math.Min(img.Width,img.Height), Math.Min(img.Width, img.Height)));
                ResultBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void DrawLines()
        {
            Graphics gfx = ResultBox.CreateGraphics();
            for (int i = 1; i <= ColumnsInput.Value; i++)
            {
                gfx.DrawLine(Pens.Black, (float)(ResultBox.Left + (i * ResultBox.Width / ColumnsInput.Value)), ResultBox.Bottom, (float)(ResultBox.Left + (i * ResultBox.Width / ColumnsInput.Value)), ResultBox.Top);
            }
        }

        private Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private void ColumnsInput_ValueChanged(object sender, EventArgs e)
        {
            DrawLines();
        }

        private void RowsInput_ValueChanged(object sender, EventArgs e)
        {
            DrawLines();
        }
    }
}
