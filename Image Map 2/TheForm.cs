using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;

// TO DO:
// catch the "editing a store map" error and yell at the user instead
// build DLLs with EXE


namespace Image_Map
{
    public partial class TheForm : Form
    {
        string LastOpenPath = "";
        string LastExportPath = "";
        string[] OpenArgs;
        List<MapPreviewBox> PicsToAdd = new List<MapPreviewBox>();
        CommonOpenFileDialog ExportDialog = new CommonOpenFileDialog()
        {
            Title = "Export your maps somewhere",
            IsFolderPicker = true,
        };
        OpenFileDialog OpenDialog = new OpenFileDialog()
        {
            Title = "Import image files to turn into maps",
            Filter = "Image Files|*.png;*.bmp;*.jpg;*.gif|All Files|*.*",
            Multiselect = true,
        };
        ImportWindow ImportDialog = new ImportWindow();
        ObservableCollection<MapPreviewBox> PicBoxes = new ObservableCollection<MapPreviewBox>();
        public TheForm(string[] args)
        {
            InitializeComponent();
            OpenArgs = args;
        }

        private void TheForm_Load(object sender, EventArgs e)
        {
            // load up saved settings
            LastOpenPath = Properties.Settings.Default.LastOpenPath;
            LastExportPath = Properties.Settings.Default.LastExportPath;
            ImportDialog.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            ImportDialog.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            BedrockCheck.Checked = Properties.Settings.Default.BedrockMode;
            AutoIDCheck.Checked = Properties.Settings.Default.AutoID;

            PicBoxes.CollectionChanged += PicBoxes_CollectionChanged;

            List<string> images = new List<string>();
            foreach (string arg in OpenArgs)
            {
                if (File.Exists(arg))
                    images.Add(arg);
            }
            if (images.Count > 0)
                ImportImages(images.ToArray());
        }

        private void PicBoxes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapIDNum_ValueChanged(null, null);
        }

        // load up images from paths, let the user modify them, and send them to processing
        private void ImportImages(string[] paths)
        {
            var images = paths.Select(x => Image.FromFile(x));
            ImportDialog.StartImports(this, images.ToList());
            Edition edition = GetCurrentEdition();
            foreach (var image in ImportDialog.OutputImages)
            {
                MapPreviewBox pic = new MapPreviewBox(image, edition)
                {
                    Width = 128,
                    Height = 128,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle
                };
                PicBoxes.Add(pic);
                PictureZone.Controls.Add(pic);
                pic.MouseDown += Pic_MouseDown;
            }
            ExportButton.Enabled = PicBoxes.Count > 0;
        }

        private Edition GetCurrentEdition()
        {
            return BedrockCheck.Checked ? Edition.Bedrock : Edition.Java;
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenDialog.InitialDirectory = LastOpenPath;
            OpenDialog.FileName = "";
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                LastOpenPath = Path.GetDirectoryName(OpenDialog.FileName);
                ImportImages(OpenDialog.FileNames);
            }
        }

        // right-click maps to remove them
        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            MapPreviewBox b = sender as MapPreviewBox;
            if (e.Button == MouseButtons.Right)
            {
                PicBoxes.Remove(b);
                PictureZone.Controls.Remove(b);
                ExportButton.Enabled = PicBoxes.Count > 0;
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // for bedrock, maps must be embedded in the world, and the worlds folder is permanent
            // therefore it is opened every time for convenience
            if (BedrockCheck.Checked)
                ExportDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
            else
                ExportDialog.InitialDirectory = LastExportPath;
            if (ExportDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                int startid = (int)MapIDNum.Value;
                LevelDB.DB bedrockdb = null;
                List<int> conflicts;
                if (BedrockCheck.Checked)
                {
                    bedrockdb = new LevelDB.DB(new LevelDB.Options(), Path.Combine(ExportDialog.FileName, "db"));
                    conflicts = MapFileSaver.CheckBedrockConflicts(bedrockdb, startid, PicBoxes.Count);
                }
                else
                    conflicts = MapFileSaver.CheckJavaConflicts(ExportDialog.FileName, startid, PicBoxes.Count);
                if (conflicts.Any() && MessageBox.Show($"Saving now will overwrite the following existing maps: {String.Join(", ", conflicts.ToArray())}\n\nWould you like to export anyway and overwrite these maps?", "Some maps will be overwritten!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                if (BedrockCheck.Checked)
                {
                    using (bedrockdb)
                    {
                        MapFileSaver.SaveBedrockMaps(PicBoxes.Select(x => x.Maps.GetBedrockMap()), bedrockdb, startid);
                        MapFileSaver.AddBedrockChests(bedrockdb, startid, PicBoxes.Count);
                    }
                }
                else
                {
                    LastExportPath = Path.GetDirectoryName(ExportDialog.FileName);
                    MapFileSaver.SaveJavaMaps(PicBoxes.Select(x => x.Maps.GetJavaMap()), ExportDialog.FileName, startid);
                    MapFileSaver.AddJavaChests(ExportDialog.FileName, startid, PicBoxes.Count);
                }
            }
        }

        private void TheForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save settings
            Properties.Settings.Default.LastExportPath = LastExportPath;
            Properties.Settings.Default.LastOpenPath = LastOpenPath;
            Properties.Settings.Default.InterpIndex = ImportDialog.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = ImportDialog.ApplyAllCheck.Checked;
            Properties.Settings.Default.BedrockMode = BedrockCheck.Checked;
            Properties.Settings.Default.AutoID = AutoIDCheck.Checked;
            Properties.Settings.Default.Save();
        }

        private void BedrockCheck_CheckedChanged(object sender, EventArgs e)
        {
            Edition edition = GetCurrentEdition();
            foreach (var box in PicBoxes)
            {
                box.ViewEdition(edition);
            }
        }

        private void AutoIDCheck_CheckedChanged(object sender, EventArgs e)
        {
            ManualIDPanel.Visible = !AutoIDCheck.Checked;
            MapIDNum_ValueChanged(sender, e);
        }

        private void MapIDNum_ValueChanged(object sender, EventArgs e)
        {
            LastIDLabel.Text = $"map_{MapIDNum.Value + Math.Max(0, PicBoxes.Count - 1)}";
        }
    }

    public enum Edition
    {
        Java,
        Bedrock
    }
}
