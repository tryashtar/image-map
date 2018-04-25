using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using fNbt;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Linq;

// TO DO:
// returning picboxes is stupid, the import thing should accept and produce images


namespace Image_Map
{
    public partial class TheForm : Form
    {
        string LastOpenPath = "";
        string LastExportPath = "";
        string[] OpenArgs;
        List<MapPreviewBox> PicsToAdd = new List<MapPreviewBox>();
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
        ImportWindow ImportDialog = new ImportWindow();
        List<MapPreviewBox> PicBoxes = new List<MapPreviewBox>();
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
            List<string> images = new List<string>();
            foreach (string arg in OpenArgs)
            {
                if (File.Exists(arg))
                    images.Add(arg);
            }
            if (images.Count > 0)
                ImportImages(images.ToArray());
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

        [HandleProcessCorruptedStateExceptions]
        private void ExportButton_Click(object sender, EventArgs e)
        {
            // for bedrock, maps must be embedded in the world, and the worlds folder is permanent
            // therefore it is opened every time for convenience
            if (BedrockCheck.Checked)
                ExportDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
            else
                ExportDialog.InitialDirectory = LastExportPath;
            ExportDialog.FileName = "map_0.dat";
            if (ExportDialog.ShowDialog() == DialogResult.OK)
            {
                if (!BedrockCheck.Checked)
                    LastExportPath = Path.GetDirectoryName(ExportDialog.FileName);
                // parse the number out of a filename like "map_53.dat" -> 53
                int.TryParse(System.Text.RegularExpressions.Regex.Match(Path.GetFileName(ExportDialog.FileName), @"\d+").Value, out int firstmapid);
                int mapid = firstmapid;
                // bedrock saving
                if (BedrockCheck.Checked)
                {
                    LevelDB.DB bedrockdb;
                    try
                    {
                        string db = Path.Combine(Path.GetDirectoryName(ExportDialog.FileName), "db");
                        if (!Directory.Exists(db))
                            throw new ApplicationException("no leveldb found");
                        bedrockdb = new LevelDB.DB(new LevelDB.Options(), db);
                    }
                    catch (ApplicationException)
                    {
                        MessageBox.Show("Bedrock Edition maps must be embedded directly in the world.\n\nPlease save your map file directly inside a Bedrock world folder.", "Not a valid world!");
                        return;
                    }
                    var file = MapHelpers.GetBedrockNbtFile(bedrockdb, "~local_player");
                    NbtCompound chestslot = null;
                    NbtList items = null;
                    byte slot = 0;
                    LevelDB.WriteBatch allmaps = new LevelDB.WriteBatch();
                    foreach (MapPreviewBox box in PicBoxes)
                    {
                        if (slot == 0)
                        {
                            // find the next empty slot in the inventory
                            // create a chest there to add maps
                            NbtList inventory = (NbtList)file.RootTag["Inventory"];
                            for (int i = 0; i < inventory.Count; i++)
                            {
                                if (inventory[i]["id"].ShortValue == 0)
                                    chestslot = (NbtCompound)inventory[i];
                            }
                            short itemslot = chestslot["Slot"].ShortValue;
                            chestslot.Clear();
                            chestslot.Add(new NbtByte("Count", 1));
                            chestslot.Add(new NbtShort("Damage", 0));
                            chestslot.Add(new NbtShort("id", 54));
                            chestslot.Add(new NbtShort("Slot", itemslot));
                            items = new NbtList("Items", NbtTagType.Compound);
                            chestslot.Add(new NbtCompound("tag") { items });
                        }
                        var map = MapHelpers.MakeBedrockMapFile(new Bitmap(box.BedrockImage), mapid);
                        allmaps.Put(map.Key, map.Value);
                        items.Add(new NbtCompound()
                        {
                            new NbtByte("Count", 1),
                            new NbtShort("Damage", 0),
                            new NbtShort("id", 358),
                            new NbtByte("Slot", slot),
                            new NbtCompound("tag") { new NbtLong("map_uuid", mapid) }
                        });
                        mapid++;
                        slot++;
                        // if the chest is full, make a new one
                        if (slot >= 27)
                            slot = 0;
                    }
                    bedrockdb.Write(allmaps);
                    bedrockdb.Dispose();
                }
                else // java saving
                {
                    foreach (MapPreviewBox box in PicBoxes)
                    {
                        MapHelpers.MakeJavaMapFile(new Bitmap(box.JavaImage)).SaveToFile(Path.Combine(Path.GetDirectoryName(ExportDialog.FileName), "map_" + mapid.ToString() + ".dat"), NbtCompression.GZip);
                        mapid++;
                    }
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

        private void BedrockCheck_CheckedChanged(object sender, EventArgs e)
        {
            Edition edition = GetCurrentEdition();
            foreach (var box in PicBoxes)
            {
                box.ViewEdition(edition);
            }
        }
    }
}
