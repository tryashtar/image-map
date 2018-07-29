using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

// TO DO:
// catch the "editing a store map" error and yell at the user instead
// hide "to map_3" label when there's less than 2 maps
// make auto-id finding work (unfortunately there seems to be no HasKey for leveldb)


namespace Image_Map
{
    public partial class TheForm : Form
    {
        string OpenJavaFolder;
        LevelDB.DB OpenBedrockWorld;
        Edition OpenEdition = Edition.None;

        string LastOpenPath = "";
        string LastExportPath = "";
        CommonOpenFileDialog SelectWorldDialog = new CommonOpenFileDialog()
        {
            Title = "Select a Minecraft world folder",
            IsFolderPicker = true,
        };
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
        List<MapPreviewBox> PicBoxes = new List<MapPreviewBox>();
        public TheForm()
        {
            InitializeComponent();
        }

        private void TheForm_Load(object sender, EventArgs e)
        {
            // load up saved settings
            LastOpenPath = Properties.Settings.Default.LastOpenPath;
            LastExportPath = Properties.Settings.Default.LastExportPath;
            ImportDialog.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            ImportDialog.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            BedrockCheck.Checked = Properties.Settings.Default.BedrockMode;
        }

        // load up images from paths, let the user modify them, and send them to processing
        private void ImportImages(string[] paths)
        {
            var images = paths.Select(x => Image.FromFile(x));
            ImportDialog.StartImports(this, images.ToList());
            foreach (var image in ImportDialog.OutputImages)
            {
                MapPreviewBox pic = new MapPreviewBox(image, Edition.None)
                {
                    Width = 128,
                    Height = 128,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle
                };
                PicBoxes.Add(pic);
                ExistingZone.Controls.Add(pic);
                pic.MouseDown += Pic_MouseDown;
            }
        }

        private void OpenWorld(string folder, Edition edition)
        {
            OpenEdition = edition;
            OpenBedrockWorld?.Dispose();
            if (edition == Edition.Java)
                OpenJavaFolder = folder;
            else if (edition==Edition.Bedrock)
                OpenBedrockWorld = new LevelDB.DB(new LevelDB.Options(), Path.Combine(folder, "db"));
            ImportZone.Controls.Clear();
            ExistingZone.Controls.Clear();

            MapFileSaver.GetBedrockMaps(OpenBedrockWorld);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (BedrockCheck.Checked)
                SelectWorldDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
            else
                SelectWorldDialog.InitialDirectory = LastExportPath;
            if (SelectWorldDialog.ShowDialog() == CommonFileDialogResult.Ok)
                OpenWorld(SelectWorldDialog.FileName, BedrockCheck.Checked ? Edition.Bedrock : Edition.Java);
        }

        // right-click maps to remove them
        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            MapPreviewBox b = sender as MapPreviewBox;
            if (e.Button == MouseButtons.Right)
            {
                PicBoxes.Remove(b);
                ExistingZone.Controls.Remove(b);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // for bedrock, maps must be embedded in the world, and the worlds folder is permanent
            // therefore it is opened every time for convenience
            if (ExportDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LevelDB.DB bedrockdb = null;
                if (BedrockCheck.Checked)
                    bedrockdb = new LevelDB.DB(new LevelDB.Options(), Path.Combine(ExportDialog.FileName, "db"));
                int startid = (int)MapIDNum.Value;
                List<int> conflicts = BedrockCheck.Checked ? MapFileSaver.CheckBedrockConflicts(bedrockdb, startid, PicBoxes.Count) : MapFileSaver.CheckJavaConflicts(ExportDialog.FileName, startid, PicBoxes.Count);
                if (conflicts.Any() && MessageBox.Show($"Saving now will overwrite the following existing maps: {String.Join(", ", conflicts.ToArray())}\n\nWould you like to export anyway and overwrite these maps?", "Some maps will be overwritten!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                if (BedrockCheck.Checked)
                {
                    using (bedrockdb)
                    {
                        //MapFileSaver.SaveBedrockMaps(PicBoxes.Select(x => x.Maps.GetBedrockMap()), bedrockdb, startid);
                        MapFileSaver.AddBedrockChests(bedrockdb, startid, PicBoxes.Count);
                    }
                }
                else
                {
                    LastExportPath = Path.GetDirectoryName(ExportDialog.FileName);
                    //MapFileSaver.SaveJavaMaps(PicBoxes.Select(x => x.Maps.GetJavaMap()), ExportDialog.FileName, startid);
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
            Properties.Settings.Default.Save();
        }

        private void MapIDNum_ValueChanged(object sender, EventArgs e)
        {
            LastIDLabel.Text = $"map_{MapIDNum.Value + Math.Max(0, PicBoxes.Count - 1)}";
        }
    }

    public enum Edition
    {
        Java,
        Bedrock,
        None
    }
}
