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
using System.Runtime.InteropServices;
using fNbt;
using System.IO;

namespace Image_Map
{
    public partial class TheForm : Form
    {
        string LastOpenPath = "";
        string LastExportPath = "";
        string[] OpenArgs;
        bool CurrentlyImporting = false;
        List<BetterPicBox> Pictures = new List<BetterPicBox>();
        List<BetterPicBox> PicsToAdd = new List<BetterPicBox>();

        SaveFileDialog ExportDialog = new SaveFileDialog()
        {
            Title = "Export your maps somewhere",
            Filter = "Map Files|*.dat|All Files|*.*",
        };
        OpenFileDialog OpenDialog = new OpenFileDialog()
        {
            Title = "Import image files to turn into maps",
            Filter = "Image Files|*.png;*.bmp;*.jpg;*.gif|All Files|*.*",
            Multiselect = true,
        };
        Dictionary<Color, byte> ColorMap;
        public TheForm(string[] args)
        {
            InitializeComponent();
            OpenArgs = args;
        }

        private void TheForm_Load(object sender, EventArgs e)
        {
            ColorMap = new Dictionary<Color, byte>()
            {
                #region color definitions
                { Color.FromArgb(88,124,39), 0x04 },
                { Color.FromArgb(108,151,47), 0x05 },
                { Color.FromArgb(126,176,55), 0x06 },
                { Color.FromArgb(66,93,29), 0x07 },
                { Color.FromArgb(172,162,114), 0x08 },
                { Color.FromArgb(210,199,138), 0x09 },
                { Color.FromArgb(244,230,161), 0x0a },
                { Color.FromArgb(128,122,85), 0x0b },
                { Color.FromArgb(138,138,138), 0x0c },
                { Color.FromArgb(169,169,169), 0x0d },
                { Color.FromArgb(197,197,197), 0x0e },
                { Color.FromArgb(104,104,104), 0x0f },
                { Color.FromArgb(178,0,0), 0x10 },
                { Color.FromArgb(217,0,0), 0x11 },
                { Color.FromArgb(252,0,0), 0x12 },
                { Color.FromArgb(133,0,0), 0x13 },
                { Color.FromArgb(111,111,178), 0x14 },
                { Color.FromArgb(136,136,217), 0x15 },
                { Color.FromArgb(158,158,252), 0x16 },
                { Color.FromArgb(83,83,133), 0x17 },
                { Color.FromArgb(116,116,116), 0x18 },
                { Color.FromArgb(142,142,142), 0x19 },
                { Color.FromArgb(165,165,165), 0x1a },
                { Color.FromArgb(87,87,87), 0x1b },
                { Color.FromArgb(0,86,0), 0x1c },
                { Color.FromArgb(0,105,0), 0x1d },
                { Color.FromArgb(0,123,0), 0x1e },
                { Color.FromArgb(0,64,0), 0x1f },
                { Color.FromArgb(178,178,178), 0x20 },
                { Color.FromArgb(217,217,217), 0x21 },
                { Color.FromArgb(252,252,252), 0x22 },
                { Color.FromArgb(133,133,133), 0x23 },
                { Color.FromArgb(114,117,127), 0x24 },
                { Color.FromArgb(139,142,156), 0x25 },
                { Color.FromArgb(162,166,182), 0x26 },
                { Color.FromArgb(85,87,96), 0x27 },
                { Color.FromArgb(105,75,53), 0x28 },
                { Color.FromArgb(128,93,65), 0x29 },
                { Color.FromArgb(149,108,76), 0x2a },
                { Color.FromArgb(78,56,40), 0x2b },
                { Color.FromArgb(78,78,78), 0x2c },
                { Color.FromArgb(95,95,95), 0x2d },
                { Color.FromArgb(111,111,111), 0x2e },
                { Color.FromArgb(58,58,58), 0x2f },
                { Color.FromArgb(44,44,178), 0x30 },
                { Color.FromArgb(54,54,217), 0x31 },
                { Color.FromArgb(63,63,252), 0x32 },
                { Color.FromArgb(33,33,133), 0x33 },
                { Color.FromArgb(99,83,49), 0x34 },
                { Color.FromArgb(122,101,61), 0x35 },
                { Color.FromArgb(141,118,71), 0x36 },
                { Color.FromArgb(74,62,38), 0x37 },
                { Color.FromArgb(178,175,170), 0x38 },
                { Color.FromArgb(217,214,209), 0x39 },
                { Color.FromArgb(252,249,242), 0x3a },
                { Color.FromArgb(133,131,127), 0x3b },
                { Color.FromArgb(150,88,36), 0x3c },
                { Color.FromArgb(184,108,43), 0x3d },
                { Color.FromArgb(213,126,50), 0x3e },
                { Color.FromArgb(113,66,27), 0x3f },
                { Color.FromArgb(124,52,150), 0x40 },
                { Color.FromArgb(151,64,184), 0x41 },
                { Color.FromArgb(176,75,213), 0x42 },
                { Color.FromArgb(93,40,113), 0x43 },
                { Color.FromArgb(71,107,150), 0x44 },
                { Color.FromArgb(87,130,184), 0x45 },
                { Color.FromArgb(101,151,213), 0x46 },
                { Color.FromArgb(53,80,113), 0x47 },
                { Color.FromArgb(159,159,36), 0x48 },
                { Color.FromArgb(195,195,43), 0x49 },
                { Color.FromArgb(226,226,50), 0x4a },
                { Color.FromArgb(120,120,27), 0x4b },
                { Color.FromArgb(88,142,17), 0x4c },
                { Color.FromArgb(108,174,21), 0x4d },
                { Color.FromArgb(126,202,25), 0x4e },
                { Color.FromArgb(66,107,13), 0x4f },
                { Color.FromArgb(168,88,115), 0x50 },
                { Color.FromArgb(206,108,140), 0x51 },
                { Color.FromArgb(239,126,163), 0x52 },
                { Color.FromArgb(126,66,86), 0x53 },
                { Color.FromArgb(52,52,52), 0x54 },
                { Color.FromArgb(64,64,64), 0x55 },
                { Color.FromArgb(75,75,75), 0x56 },
                { Color.FromArgb(40,40,40), 0x57 },
                { Color.FromArgb(107,107,107), 0x58 },
                { Color.FromArgb(130,130,130), 0x59 },
                { Color.FromArgb(151,151,151), 0x5a },
                { Color.FromArgb(80,80,80), 0x5b },
                { Color.FromArgb(52,88,107), 0x5c },
                { Color.FromArgb(64,108,130), 0x5d },
                { Color.FromArgb(75,126,151), 0x5e },
                { Color.FromArgb(40,66,80), 0x5f },
                { Color.FromArgb(88,43,124), 0x60 },
                { Color.FromArgb(108,53,151), 0x61 },
                { Color.FromArgb(126,62,176), 0x62 },
                { Color.FromArgb(66,33,93), 0x63 },
                { Color.FromArgb(36,52,124), 0x64 },
                { Color.FromArgb(43,64,151), 0x65 },
                { Color.FromArgb(50,75,176), 0x66 },
                { Color.FromArgb(27,40,93), 0x67 },
                { Color.FromArgb(71,52,36), 0x68 },
                { Color.FromArgb(87,64,43), 0x69 },
                { Color.FromArgb(101,75,50), 0x6a },
                { Color.FromArgb(53,40,27), 0x6b },
                { Color.FromArgb(71,88,36), 0x6c },
                { Color.FromArgb(87,108,43), 0x6d },
                { Color.FromArgb(101,126,50), 0x6e },
                { Color.FromArgb(53,66,27), 0x6f },
                { Color.FromArgb(107,36,36), 0x70 },
                { Color.FromArgb(130,43,43), 0x71 },
                { Color.FromArgb(151,50,50), 0x72 },
                { Color.FromArgb(80,27,27), 0x73 },
                { Color.FromArgb(17,17,17), 0x74 },
                { Color.FromArgb(21,21,21), 0x75 },
                { Color.FromArgb(25,25,25), 0x76 },
                { Color.FromArgb(13,13,13), 0x77 },
                { Color.FromArgb(174,166,53), 0x78 },
                { Color.FromArgb(212,203,65), 0x79 },
                { Color.FromArgb(247,235,76), 0x7a },
                { Color.FromArgb(130,125,40), 0x7b },
                { Color.FromArgb(63,152,148), 0x7c },
                { Color.FromArgb(78,186,181), 0x7d },
                { Color.FromArgb(91,216,210), 0x7e },
                { Color.FromArgb(47,114,111), 0x7f },
                { Color.FromArgb(51,89,178), 0x80 },
                { Color.FromArgb(62,109,217), 0x81 },
                { Color.FromArgb(73,126,252), 0x82 },
                { Color.FromArgb(39,66,133), 0x83 },
                { Color.FromArgb(0,151,40), 0x84 },
                { Color.FromArgb(0,185,49), 0x85 },
                { Color.FromArgb(0,214,57), 0x86 },
                { Color.FromArgb(0,113,30), 0x87 },
                { Color.FromArgb(90,59,34), 0x88 },
                { Color.FromArgb(110,73,42), 0x89 },
                { Color.FromArgb(127,85,48), 0x8a },
                { Color.FromArgb(67,44,25), 0x8b },
                { Color.FromArgb(78,1,0), 0x8c },
                { Color.FromArgb(95,1,0), 0x8d },
                { Color.FromArgb(111,2,0), 0x8e },
                { Color.FromArgb(58,1,0), 0x8f },
                { Color.FromArgb(145,123,112), 0x90 },
                { Color.FromArgb(178,150,136), 0x91 },
                { Color.FromArgb(207,175,159), 0x92 },
                { Color.FromArgb(109,92,84), 0x93 },
                { Color.FromArgb(111,56,25), 0x94 },
                { Color.FromArgb(135,69,31), 0x95 },
                { Color.FromArgb(157,81,36), 0x96 },
                { Color.FromArgb(83,42,19), 0x97 },
                { Color.FromArgb(104,60,75), 0x98 },
                { Color.FromArgb(126,74,92), 0x99 },
                { Color.FromArgb(147,86,107), 0x9a },
                { Color.FromArgb(77,45,56), 0x9b },
                { Color.FromArgb(78,75,96), 0x9c },
                { Color.FromArgb(95,92,118), 0x9d },
                { Color.FromArgb(111,107,136), 0x9e },
                { Color.FromArgb(58,56,72), 0x9f },
                { Color.FromArgb(129,92,25), 0xa0 },
                { Color.FromArgb(158,113,31), 0xa1 },
                { Color.FromArgb(184,131,36), 0xa2 },
                { Color.FromArgb(97,69,19), 0xa3 },
                { Color.FromArgb(71,81,37), 0xa4 },
                { Color.FromArgb(87,99,44), 0xa5 },
                { Color.FromArgb(102,116,52), 0xa6 },
                { Color.FromArgb(53,60,28), 0xa7 },
                { Color.FromArgb(111,53,54), 0xa8 },
                { Color.FromArgb(136,65,66), 0xa9 },
                { Color.FromArgb(158,76,77), 0xaa },
                { Color.FromArgb(83,40,41), 0xab },
                { Color.FromArgb(40,28,24), 0xac },
                { Color.FromArgb(48,35,30), 0xad },
                { Color.FromArgb(56,41,35), 0xae },
                { Color.FromArgb(30,21,18), 0xaf },
                { Color.FromArgb(94,74,68), 0xb0 },
                { Color.FromArgb(115,91,83), 0xb1 },
                { Color.FromArgb(133,106,97), 0xb2 },
                { Color.FromArgb(70,55,50), 0xb3 },
                { Color.FromArgb(60,63,63), 0xb4 },
                { Color.FromArgb(74,78,78), 0xb5 },
                { Color.FromArgb(86,91,91), 0xb6 },
                { Color.FromArgb(45,47,47), 0xb7 },
                { Color.FromArgb(85,50,61), 0xb8 },
                { Color.FromArgb(104,61,74), 0xb9 },
                { Color.FromArgb(121,72,87), 0xba },
                { Color.FromArgb(63,38,45), 0xbb },
                { Color.FromArgb(52,42,63), 0xbc },
                { Color.FromArgb(64,52,78), 0xbd },
                { Color.FromArgb(75,61,91), 0xbe },
                { Color.FromArgb(40,32,47), 0xbf },
                { Color.FromArgb(52,35,24), 0xc0 },
                { Color.FromArgb(64,42,30), 0xc1 },
                { Color.FromArgb(75,49,35), 0xc2 },
                { Color.FromArgb(40,26,18), 0xc3 },
                { Color.FromArgb(52,56,29), 0xc4 },
                { Color.FromArgb(64,69,36), 0xc5 },
                { Color.FromArgb(75,81,42), 0xc6 },
                { Color.FromArgb(40,42,22), 0xc7 },
                { Color.FromArgb(99,42,32), 0xc8 },
                { Color.FromArgb(121,50,39), 0xc9 },
                { Color.FromArgb(140,59,45), 0xca },
                { Color.FromArgb(74,31,24), 0xcb },
                { Color.FromArgb(26,15,11), 0xcc },
                { Color.FromArgb(31,18,13), 0xcd },
                { Color.FromArgb(37,22,16), 0xce },
                { Color.FromArgb(19,11,8), 0xcf },
                #endregion
            };
            PixelRadio.Checked = Properties.Settings.Default.PrefersPixelArt;
            NormalRadio.Checked = !PixelRadio.Checked;
            SplitRadio.Checked = Properties.Settings.Default.PrefersSplitting;
            ScaleRadio.Checked = !SplitRadio.Checked;
            LastOpenPath = Properties.Settings.Default.LastOpenPath;
            LastExportPath = Properties.Settings.Default.LastExportPath;
            List<string> images = new List<string>();
            foreach (string arg in OpenArgs)
            {
                if (File.Exists(arg))
                    images.Add(arg);
            }
            if (images.Count > 0)
                ImportImages(images.ToArray());
        }

        public Image Mapify(Image img)
        {
            Bitmap map = new Bitmap(img);
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixelcolor = map.GetPixel(i, j);
                    if (pixelcolor.A < 128)
                    {
                        map.SetPixel(i, j, Color.FromArgb(0, 0, 0, 0));
                        continue;
                    }
                    int index = 0;
                    double[] diffs = new double[ColorMap.Keys.Count];
                    foreach (Color mapcolor in ColorMap.Keys)
                    {
                        diffs[index] = ColorDistance(pixelcolor, mapcolor);
                        index++;
                    }
                    map.SetPixel(i, j, ColorMap.Keys.ElementAt(diffs.ToList().IndexOf(diffs.Min())));
                }
            }
            return map;
        }

        public double ColorDistance(Color e1, Color e2)
        {
            long rmean = ((long)e1.R + (long)e2.R) / 2;
            long r = (long)e1.R - (long)e2.R;
            long g = (long)e1.G - (long)e2.G;
            long b = (long)e1.B - (long)e2.B;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }

        private void ImportImages(string[] paths)
        {
            if (CurrentlyImporting)
                return;
            CurrentlyImporting = true;
            List<BetterPicBox> newboxes = new List<BetterPicBox>();
            foreach (string path in paths)
            {
                Image img = Image.FromFile(path);
                List<Image> images = new List<Image>();
                if (SplitRadio.Checked)
                {
                    SplitImageForm split = new SplitImageForm();
                    split.PicturePreview.Image = img;
                    split.ShowDialog(this);
                    for (int y = 0; y < split.HeightInput.Value; y++)
                    {
                        for (int x = 0; x < split.WidthInput.Value; x++)
                        {
                            images.Add(CropImage(img, new Rectangle(
                                (int)(x * img.Width / split.WidthInput.Value),
                                (int)(y * img.Height / split.HeightInput.Value),
                                (int)(img.Width / split.WidthInput.Value),
                                (int)(img.Height / split.HeightInput.Value))));
                        }
                    }
                }
                else
                    images.Add(img);
                foreach (Image image in images)
                {
                    BetterPicBox pic = new BetterPicBox(null, ResizeImg(image, 128, 128, PixelRadio.Checked ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic))
                    {
                        Width = 128,
                        Height = 128,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    Pictures.Add(pic);
                    newboxes.Add(pic);
                    pic.MouseDown += Pic_MouseDown;
                }
            }
            OpenButton.Enabled = false;
            // mapify the new images in the background
            ImportProcessor.RunWorkerAsync(newboxes);
        }

        private Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, PixelFormat.DontCare);
            return (Image)(bmpCrop);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenDialog.InitialDirectory = LastOpenPath;
            OpenDialog.FileName = "";
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                LastOpenPath = OpenDialog.FileName;
                ImportBar.Visible = true;
                ImportLabel.Visible = true;
                ImportImages(OpenDialog.FileNames);
            }
        }

        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            BetterPicBox b = sender as BetterPicBox;
            if (e.Button == MouseButtons.Right)
            {
                Pictures.Remove(b);
                PictureZone.Controls.Remove(b);
                PictureZone_Resize(null, null);
                ExportButton.Enabled = Pictures.Count > 0;
            }
        }

        private void PictureZone_Resize(object sender, EventArgs e)
        {
            int x = 10;
            int y = 10;
            foreach (BetterPicBox box in Pictures)
            {
                if (x + box.Width > PictureZone.Width)
                {
                    x = 10;
                    y += box.Height + 10;
                }
                box.Left = x;
                box.Top = y - PictureZone.VerticalScroll.Value;
                x += box.Width + 10;
            }
            MosaicPanel.Visible = Pictures.Count > 1;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            ExportDialog.InitialDirectory = LastExportPath;
            ExportDialog.FileName = "map_0.dat";
            if (ExportDialog.ShowDialog() == DialogResult.OK)
            {
                LastExportPath = ExportDialog.FileName;
                string name = Path.GetFileName(ExportDialog.FileName);
                int.TryParse(System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value, out int index);
                int height = (int)Math.Ceiling((double)Pictures.Count / (double)MosaicWidth.Value);
                StringBuilder generatecommand = new StringBuilder("summon falling_block ~ ~.6 ~ {Time:1,Block:repeating_command_block,TileEntityData:{auto:1,Command:\"setblock ~ ~1 ~ activator_rail powered=true\"},Passengers:[{id:commandblock_minecart,Command:\"fill ~ ~-2 ~1 ~" + (MosaicWidth.Value - 1) + " ~" + (height - 3) + " ~1 sea_lantern\"}");
                int x = 0;
                int y = height;
                foreach (BetterPicBox box in Pictures)
                {
                    Bitmap bmp = new Bitmap(box.MainImage);
                    byte[] mapbytes = new byte[128 * 128];
                    for (int i = 0; i < 128; i++)
                    {
                        for (int j = 0; j < 128; j++)
                        {
                            Color pixel = bmp.GetPixel(j, i);
                            if (pixel == Color.FromArgb(0, 0, 0, 0))
                                mapbytes[128 * i + j] = 0x00;
                            else
                                mapbytes[128 * i + j] = ColorMap[pixel];
                        }
                    }
                    NbtCompound map = new NbtCompound("map")
                    {
                        new NbtCompound("data")
                        {
                            new NbtByte("scale",0),
                            new NbtByte("dimension",0),
                            new NbtShort("height",128),
                            new NbtShort("width",128),
                            new NbtByte("trackingPosition",0),
                            new NbtByte("unlimitedTracking",0),
                            new NbtInt("xCenter",2147483647),
                            new NbtInt("zCenter",2147483647),
                            new NbtByteArray("colors",mapbytes)
                        }
                    };
                    NbtFile file = new NbtFile(map);
                    file.SaveToFile(Path.Combine(Path.GetDirectoryName(ExportDialog.FileName), "map_" + index.ToString() + ".dat"), NbtCompression.GZip);
                    generatecommand.Append(",{id:commandblock_minecart,Command:\"summon item_frame ~" + x + " ~" + (y - 3) + " ~2 {Invulnerable:1b,Silent:1b,Item:{id:filled_map,Count:1b,Damage:" + index + "s}}\"}");
                    index++;
                    x++;
                    if (x > MosaicWidth.Value - 1)
                    {
                        x = 0;
                        y--;
                    }
                }
                generatecommand.Append(",{id:commandblock_minecart,Command:\"blockdata ~ ~-1 ~ {Command:\\\"fill ~ ~-1 ~ ~ ~1 ~ air\\\",auto:1}\"},{id:commandblock_minecart,Command:\"kill @e[type=commandblock_minecart,r=1]\"}]}");
                MosaicOutputBox.Text = generatecommand.ToString();
                MosaicOutputBox.Visible = true;
                MosaicOutputCopy.Visible = true;
            }
        }

        public Image ResizeImg(Image image, int width, int height, InterpolationMode mode)
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

        private void TheForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.LastExportPath = LastExportPath;
            Properties.Settings.Default.LastOpenPath = LastOpenPath;
            Properties.Settings.Default.PrefersPixelArt = PixelRadio.Checked;
            Properties.Settings.Default.PrefersSplitting = SplitRadio.Checked;
            Properties.Settings.Default.Save();
        }

        private void ImportProcessor_DoWork(object sender, DoWorkEventArgs e)
        {
            int prog = 0;
            foreach (BetterPicBox pic in (List<BetterPicBox>)e.Argument)
            {
                pic.MainImage = Mapify(pic.HoverImage);
                prog++;
                ImportProcessor.ReportProgress(prog * 100 / ((List<BetterPicBox>)e.Argument).Count);
            }
            PicsToAdd = (List<BetterPicBox>)e.Argument;
        }

        private void ImportProcessor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ImportBar.Value = Math.Min(100, e.ProgressPercentage);
            ImportLabel.Text = "Mapifying... (" + e.ProgressPercentage.ToString() + "%)";
        }

        private void ImportProcessor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (BetterPicBox box in PicsToAdd)
            {
                box.Image = box.MainImage;
            }
            PictureZone.Controls.AddRange(PicsToAdd.ToArray());
            PictureZone_Resize(null, null);
            ImportBar.Visible = false;
            ImportLabel.Visible = false;
            OpenButton.Enabled = true;
            ExportButton.Enabled = true;
            CurrentlyImporting = false;
        }

        private void MosaicOutputCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MosaicOutputBox.Text);
        }
    }

    public class BetterPicBox : PictureBox
    {
        public Image MainImage;
        public Image HoverImage;
        public BetterPicBox(Image main, Image hover)
        {
            MainImage = main;
            HoverImage = hover;
            Image = MainImage;
            MouseEnter += BetterPicBox_MouseEnter;
            MouseLeave += BetterPicBox_MouseLeave;
        }

        private void BetterPicBox_MouseLeave(object sender, EventArgs e)
        {
            Image = MainImage;
        }

        private void BetterPicBox_MouseEnter(object sender, EventArgs e)
        {
            Image = HoverImage;
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            base.OnPaint(paintEventArgs);
        }
    }
}
