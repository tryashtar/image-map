using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public partial class ImportWindow : Form
    {
        int EditingIndex = 0;
        private List<Image> InputImages;
        public List<Bitmap> OutputImages = new List<Bitmap>();
        RotateFlipType Rotation = RotateFlipType.RotateNoneFlipNone;
        public ImportWindow()
        {
            InitializeComponent();
            InterpolationModeBox.SelectedIndex = 0;
        }

        public void StartImports(Form parent, List<Image> images)
        {
            InputImages = images;
            OutputImages.Clear();
            EditingIndex = 0;
            PreviewBox.Image = InputImages[0];
            PreviewBox.Interp = GetInterpolationMode();
            CurrentIndexLabel.Text = $"1 / {InputImages.Count}";
            CurrentIndexLabel.Visible = (InputImages.Count > 1);
            ApplyAllCheck.Visible = (InputImages.Count > 1);
            ShowDialog(parent);
        }

        private void ProcessNextImage()
        {
            EditingIndex++;
            if (EditingIndex >= InputImages.Count)
                Finish();
            else
            {
                Rotation = RotateFlipType.RotateNoneFlipNone;
                PreviewBox.Image = InputImages[EditingIndex];
                PreviewBox.Interp = GetInterpolationMode();
                InputImages[EditingIndex].RotateFlip(Rotation);
                CurrentIndexLabel.Text = $"{EditingIndex + 1} / {InputImages.Count}";
            }
        }

        private void Finish()
        {
            this.Close();
            InputImages = null;
        }

        private void DimensionsInput_ValueChanged(object sender, EventArgs e)
        {
            PreviewBox.Width = (int)((double)WidthInput.Value / (double)HeightInput.Value * PreviewBox.Height);
            PreviewBox.Left = this.Width / 2 - (PreviewBox.Width / 2);
        }

        private void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            Pen black = new Pen(Color.Black, 3f);
            Pen white = new Pen(Color.White, 1f);
            for (int i = 1; i < WidthInput.Value; i++)
            {
                int split = (int)((double)PreviewBox.Width / (double)WidthInput.Value * i);
                e.Graphics.DrawLine(black, new Point(split, 0), new Point(split, PreviewBox.Height));
                e.Graphics.DrawLine(white, new Point(split, 0), new Point(split, PreviewBox.Height));
            }
            for (int i = 1; i < HeightInput.Value; i++)
            {
                int split = (int)((double)PreviewBox.Height / (double)HeightInput.Value * i);
                e.Graphics.DrawLine(black, new Point(0, split), new Point(PreviewBox.Width, split));
                e.Graphics.DrawLine(white, new Point(0, split), new Point(PreviewBox.Width, split));
            }
        }

        private InterpolationMode GetInterpolationMode()
        {
            if (InterpolationModeBox.SelectedIndex == 1)
                return InterpolationMode.NearestNeighbor;
            else if (InterpolationModeBox.SelectedIndex == 2)
                return InterpolationMode.HighQualityBicubic;
            else // automatic
                return (InputImages[EditingIndex].Height > 128 && InputImages[EditingIndex].Width > 128) ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
        }

        private void InterpolationModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewBox.Image == null)
                return;
            PreviewBox.Interp = GetInterpolationMode();
        }

        private static Bitmap CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, PixelFormat.DontCare);
        }

        private static Bitmap ResizeImg(Image image, int width, int height, InterpolationMode mode)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = mode;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            int index = EditingIndex;
            int final = ApplyAllCheck.Checked ? InputImages.Count - 1 : EditingIndex;
            for (int i = index; i <= final; i++)
            {
                if (i > index)
                    InputImages[i].RotateFlip(Rotation);
                Bitmap img = ResizeImg(InputImages[i], (int)(128 * WidthInput.Value), (int)(128 * HeightInput.Value), GetInterpolationMode());
                for (int y = 0; y < HeightInput.Value; y++)
                {
                    for (int x = 0; x < WidthInput.Value; x++)
                    {
                        OutputImages.Add(CropImage(img, new Rectangle(
                            (int)(x * img.Width / WidthInput.Value),
                            (int)(y * img.Height / HeightInput.Value),
                            (int)(img.Width / WidthInput.Value),
                            (int)(img.Height / HeightInput.Value))));
                    }
                }
            }
            EditingIndex = final;
            ProcessNextImage();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (ApplyAllCheck.Checked)
                Finish();
            else
                ProcessNextImage();
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            if (Rotation == RotateFlipType.RotateNoneFlipNone)
                Rotation = RotateFlipType.Rotate90FlipNone;
            else if (Rotation == RotateFlipType.Rotate90FlipNone)
                Rotation = RotateFlipType.Rotate180FlipNone;
            else if (Rotation == RotateFlipType.Rotate180FlipNone)
                Rotation = RotateFlipType.Rotate270FlipNone;
            else if (Rotation == RotateFlipType.Rotate270FlipNone)
                Rotation = RotateFlipType.RotateNoneFlipNone;
            InputImages[EditingIndex].RotateFlip(RotateFlipType.Rotate90FlipNone);
            PreviewBox.Refresh();
        }
    }
}
