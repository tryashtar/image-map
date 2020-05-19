using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    public partial class ImportWindow : Form
    {
        private bool AllowDither = false;
        private bool Finished = false;
        private int EditingIndex = 0;
        private string[] InputPaths;
        private Bitmap CurrentImage;
        public bool DitherChecked { get { return DitherCheck.Checked; } set { DitherCheck.Checked = value; } }
        public bool StretchChecked { get { return StretchCheck.Checked; } set { StretchCheck.Checked = value; } }
        public event EventHandler<MapCreationSettings> ImageReady;
        RotateFlipType Rotation = RotateFlipType.RotateNoneFlipNone;
        public ImportWindow(bool allowdither)
        {
            InitializeComponent();
            InterpolationModeBox.SelectedIndex = 0;
            AllowDither = allowdither;
            DitherCheck.Visible = allowdither;
        }

        public void StartImports(Form parent, string[] inputpaths)
        {
            InputPaths = inputpaths;
            CurrentIndexLabel.Visible = (InputPaths.Length > 1);
            ApplyAllCheck.Visible = (InputPaths.Length > 1);
            EditingIndex = -1;
            ProcessNextImage(false);
            if (!Finished) // don't try to show if all loaded images were skipped
                ShowDialog(parent);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                ConfirmButton_Click(this, new EventArgs());
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ProcessNextImage(bool invisible)
        {
            EditingIndex++;
            if (EditingIndex >= InputPaths.Length)
            {
                Finish();
                return;
            }
            string filename = Path.GetFileName(InputPaths[EditingIndex]);
            try
            {
                CurrentImage = new Bitmap(InputPaths[EditingIndex]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The image {filename} could not be loaded\n\nExact error message: {ex.Message}", "Bad image!");
                ProcessNextImage(invisible);
                return;
            }
            if (!invisible)
            {
                Rotation = RotateFlipType.RotateNoneFlipNone;
                PreviewBox.Image = CurrentImage;
                this.Text = "Import – " + filename;
                PreviewBox.Interp = GetInterpolationMode();
                CurrentIndexLabel.Text = $"{EditingIndex + 1} / {InputPaths.Length}";
            }
            CurrentImage.RotateFlip(Rotation);
        }

        private void Finish()
        {
            Finished = true;
            InputPaths = null;
            CurrentImage = null;
            this.Close();
        }

        private void DimensionsInput_ValueChanged(object sender, EventArgs e)
        {
            ProportionPreview();
            PreviewBox.Refresh();
        }

        private void StretchCheck_CheckedChanged(object sender, EventArgs e)
        {
            ProportionPreview();
        }

        private void ImportWindow_Layout(object sender, LayoutEventArgs e)
        {
            ProportionPreview();
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
                return (CurrentImage.Height > 128 && CurrentImage.Width > 128) ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
        }

        private void InterpolationModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewBox.Image == null)
                return;
            PreviewBox.Interp = GetInterpolationMode();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            int index = EditingIndex;
            int final = ApplyAllCheck.Checked ? InputPaths.Length - 1 : EditingIndex;
            for (int i = index; i <= final; i++)
            {
                if (i > index)
                    ProcessNextImage(true);
                if (CurrentImage == null)
                    return;
                bool dithered = AllowDither && DitherChecked;
                var settings = new MapCreationSettings(CurrentImage, (int)WidthInput.Value, (int)HeightInput.Value, GetInterpolationMode(), dithered, StretchChecked);
                ImageReady?.Invoke(this, settings);
            }
            EditingIndex = final;
            ProcessNextImage(false);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (ApplyAllCheck.Checked)
                Finish();
            else
                ProcessNextImage(false);
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
            CurrentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            PreviewBox.Refresh();
        }

        private void ProportionPreview()
        {
            double ideal = (double)HeightInput.Value / (double)WidthInput.Value;
            PreviewBox.SizeMode = StretchChecked ? PictureBoxSizeMode.StretchImage : PictureBoxSizeMode.Zoom;
            PreviewBox.Height = (int)Math.Min(PreviewPanel.Height, ideal * PreviewPanel.Width);
            PreviewBox.Width = (int)Math.Min(PreviewPanel.Width, PreviewPanel.Height / ideal);
            PreviewBox.Left = PreviewPanel.Width / 2 - (PreviewBox.Width / 2);
            PreviewBox.Top = PreviewPanel.Height / 2 - (PreviewBox.Height / 2);
        }
    }
}
