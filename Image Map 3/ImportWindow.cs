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
        private bool ApproximateColorOptions = false;
        private bool Finished = false;
        private int EditingIndex = 0;
        private bool SingleImage = false;
        private string[] InputPaths;
        private Image CurrentImage;
        public bool DitherChecked { get { return DitherCheck.Checked; } set { DitherCheck.Checked = value; } }
        public bool StretchChecked { get { return StretchCheck.Checked; } set { StretchCheck.Checked = value; } }
        public event EventHandler<MapCreationSettings> ImageReady;
        RotateFlipType Rotation = RotateFlipType.RotateNoneFlipNone;
        public ImportWindow(bool approximate_colors)
        {
            InitializeComponent();
            ApproximateColorOptions = approximate_colors;
            DitherCheck.Visible = ApproximateColorOptions;
            ColorAlgorithmBox.Visible = ApproximateColorOptions;
            ColorAlgorithmBox.Items.Add(new ColorAlgorithmDisplay("Good Fast Algorithm", SimpleAlgorithm.Instance));
            ColorAlgorithmBox.Items.Add(new ColorAlgorithmDisplay("Euclidean Algorithm", EuclideanAlgorithm.Instance));
            ColorAlgorithmBox.Items.Add(new ColorAlgorithmDisplay("CIEDE2000 Algorithm", Ciede2000Algorithm.Instance));
            ColorAlgorithmBox.Items.Add(new ColorAlgorithmDisplay("CIE76 Algorithm", Cie76Algorithm.Instance));
            ColorAlgorithmBox.Items.Add(new ColorAlgorithmDisplay("CMC Algorithm", CmcAlgorithm.Instance));
            InterpolationModeBox.Items.Add(new ScalingModeDisplay("Automatic Scaling", ScalingMode.Automatic));
            InterpolationModeBox.Items.Add(new ScalingModeDisplay("Pixel Art Scaling", ScalingMode.NearestNeighbor));
            InterpolationModeBox.Items.Add(new ScalingModeDisplay("Bicubic Scaling", ScalingMode.Bicubic));
        }

        public void StartImports(Form parent, IEnumerable<string> inputpaths)
        {
            InputPaths = inputpaths.ToArray();
            CurrentIndexLabel.Visible = (InputPaths.Length > 1);
            ConfirmAllButton.Visible = (InputPaths.Length > 1);
            CancelAllButton.Visible = (InputPaths.Length > 1);
            EditingIndex = -1;
            ProcessNextImage(false);
            if (!Finished) // don't try to show if all loaded images were skipped
                ShowDialog(parent);
        }

        public void StartImports(Form parent, Image imagedata)
        {
            ProcessImage(imagedata);
            ShowDialog(parent);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                ConfirmButton_Click(this, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Escape)
            {
                CancelAllButton_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ProcessImage(Image image)
        {
            SingleImage = true;
            CurrentImage = image;
            CurrentIndexLabel.Visible = false;
            ConfirmAllButton.Visible = false;
            CancelAllButton.Visible = false;
            Rotation = RotateFlipType.RotateNoneFlipNone;
            PreviewBox.Image = image;
            this.Text = "Import image data";
            PreviewBox.Interp = GetInterpolationMode();
            image.RotateFlip(Rotation);
        }

        private void ProcessNextImage(bool invisible)
        {
            EditingIndex++;
            if (SingleImage || EditingIndex >= InputPaths.Length)
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
            var scaling = ((ScalingModeDisplay)InterpolationModeBox.SelectedItem).Mode;
            switch (scaling)
            {
                case ScalingMode.Bicubic:
                    return InterpolationMode.HighQualityBicubic;
                case ScalingMode.NearestNeighbor:
                    return InterpolationMode.NearestNeighbor;
                case ScalingMode.Automatic:
                    return (CurrentImage.Height > 128 && CurrentImage.Width > 128) ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
                default:
                    throw new ArgumentException();
            }
        }

        private IColorAlgorithm GetAlgorithm()
        {
            return ((ColorAlgorithmDisplay)ColorAlgorithmBox.SelectedItem).Algorithm;
        }

        private void InterpolationModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewBox.Image == null)
                return;
            PreviewBox.Interp = GetInterpolationMode();
        }

        private void ConfirmUntil(int final_index)
        {
            int index = EditingIndex;
            for (int i = index; i <= final_index; i++)
            {
                if (i > index)
                    ProcessNextImage(true);
                if (CurrentImage == null)
                    return;
                bool dithered = ApproximateColorOptions && DitherChecked;
                var settings = new MapCreationSettings(CurrentImage, (int)WidthInput.Value, (int)HeightInput.Value, GetInterpolationMode(), dithered, StretchChecked, GetAlgorithm());
                ImageReady?.Invoke(this, settings);
            }
            EditingIndex = final_index;
            ProcessNextImage(false);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            ConfirmUntil(EditingIndex);
        }

        private void ConfirmAllButton_Click(object sender, EventArgs e)
        {
            ConfirmUntil(InputPaths.Length - 1);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ProcessNextImage(false);
        }

        private void CancelAllButton_Click(object sender, EventArgs e)
        {
            Finish();
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

    public class ColorAlgorithmDisplay
    {
        public readonly string Name;
        public readonly IColorAlgorithm Algorithm;
        public ColorAlgorithmDisplay(string name, IColorAlgorithm algorithm)
        {
            Name = name;
            Algorithm = algorithm;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum ScalingMode
    {
        Bicubic,
        NearestNeighbor,
        Automatic
    }

    public class ScalingModeDisplay
    {
        public readonly string Name;
        public readonly ScalingMode Mode;
        public ScalingModeDisplay(string name, ScalingMode mode)
        {
            Name = name;
            Mode = mode;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
