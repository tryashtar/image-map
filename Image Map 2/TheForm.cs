using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace Image_Map
{
    public partial class TheForm : Form
    {
        MinecraftWorld SelectedWorld;
        string LastOpenPath = "";
        string LastWorldPath = "";
        string LastImgExportPath = "";
        string BedrockSavesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
        CommonOpenFileDialog SelectWorldDialog = new CommonOpenFileDialog()
        {
            Title = "Select a Minecraft world folder",
            IsFolderPicker = true,
        };
        SaveFileDialog ExportDialog = new SaveFileDialog()
        {
            Title = "Export this map as a PNG",
            Filter = "Image Files|*.png|All Files|*.*"
        };
        OpenFileDialog OpenDialog = new OpenFileDialog()
        {
            Title = "Import image files to turn into maps",
            Filter = "Image Files|*.png;*.bmp;*.jpg;*.gif|All Files|*.*",
            Multiselect = true,
        };
        ImportWindow ImportDialog = new ImportWindow();
        BedrockWorldWindow WorldDialog = new BedrockWorldWindow();
        List<MapIDControl> ImportingMaps = new List<MapIDControl>();
        List<MapIDControl> ExistingMaps = new List<MapIDControl>();
        public TheForm()
        {
            InitializeComponent();
        }

        private void TheForm_Load(object sender, EventArgs e)
        {
            // load up saved settings
            LastOpenPath = Properties.Settings.Default.LastOpenPath;
            LastWorldPath = Properties.Settings.Default.LastWorldPath;
            LastImgExportPath = Properties.Settings.Default.LastImgExportPath;
            ImportDialog.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            ImportDialog.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            AddChestCheck.Checked = Properties.Settings.Default.AddChest;
        }

        private void NewWorldOpened()
        {
            MapView.Visible = true;
            ImportingMaps.Clear();
            ImportZone.Controls.Clear();
            ExistingMaps.Clear();
            ExistingZone.Controls.Clear();
            foreach (var map in SelectedWorld.WorldMaps.OrderBy(x => x.Key))
            {
                MapIDControl mapbox = new MapIDControl(map.Key, map.Value);
                ExistingMaps.Add(mapbox);
                ExistingZone.Controls.Add(mapbox);
            }
            WorldNameLabel.Text = SelectedWorld.Name;
        }

        private void JavaWorldButton_Click(object sender, EventArgs e)
        {
            if (ImportingMaps.Any() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you open a new world, these maps will disappear.\n\nWould you like to close this world anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
                SelectWorldDialog.InitialDirectory = LastWorldPath;
            if (SelectWorldDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (!ImportingMaps.Any() || MessageBox.Show("You have unsaved maps waiting to be imported! If you select a new world, these will be lost!\n\nDiscard unsaved maps?", "Wait a minute!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LastWorldPath = SelectWorldDialog.FileName;
                    try
                    {
                        SelectedWorld = new JavaWorld(SelectWorldDialog.FileName);
                        NewWorldOpened();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Encountered the following error while loading this world:\n\n{ex.Message}", "Ouchie ouch");
                    }
                }
            }
        }


        private void BedrockWorldButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(BedrockSavesFolder))
            {
                MessageBox.Show("You seemingly don't have Bedrock Edition installed.", "Hmm...");
                return;
            }
            if (ImportingMaps.Any() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you open a new world, these maps will disappear.\n\nWould you like to close this world anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            WorldDialog.ShowWorlds(this, BedrockSavesFolder);
            if (WorldDialog.Confirmed)
            {
                try
                {
                    SelectedWorld = new BedrockWorld(WorldDialog.SelectedWorldFolder);
                    NewWorldOpened();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Encountered the following error while loading this world:\n\n{ex.Message}", "Ouchie ouch");
                }
            }
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenDialog.InitialDirectory = LastOpenPath;
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                LastOpenPath = Path.GetDirectoryName(OpenDialog.FileName);
                var images = OpenDialog.FileNames.Select(x => Image.FromFile(x));
                ImportDialog.StartImports(this, images.ToList());
                var taken = ImportingMaps.Select(x => x.ID).Concat(ExistingMaps.Select(x => x.ID)).ToList();
                taken.Add(-1);
                long id = taken.Max() + 1;
                foreach (var image in ImportDialog.OutputImages)
                {
                    MapIDControl mapbox = new MapIDControl(id, SelectedWorld is JavaWorld ? (Map)new JavaMap(image) : new BedrockMap(image));
                    ImportingMaps.Add(mapbox);
                    ImportZone.Controls.Add(mapbox);
                    mapbox.MouseDown += ImportingBox_MouseDown;
                    id++;
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SelectedWorld.AddMaps(ImportingMaps.ToDictionary(x => x.ID, x => x.Map));
            if (AddChestCheck.Checked)
                SelectedWorld.AddChestsLocalPlayer(ImportingMaps.Select(x => x.ID));
            ExistingMaps.AddRange(ImportingMaps);
            ExistingZone.Controls.AddRange(ImportingMaps.ToArray());
            ImportingMaps.Clear();
            ImportZone.Controls.Clear();
        }

        // right-click maps to remove them
        private void ImportingBox_MouseDown(object sender, MouseEventArgs e)
        {
            MapIDControl box = sender as MapIDControl;
            if (e.Button == MouseButtons.Right)
            {
                ImportingMaps.Remove(box);
                ImportZone.Controls.Remove(box);
            }
        }

        private void TheForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save settings
            Properties.Settings.Default.LastWorldPath = LastWorldPath;
            Properties.Settings.Default.LastOpenPath = LastOpenPath;
            Properties.Settings.Default.LastImgExportPath = LastImgExportPath;
            Properties.Settings.Default.InterpIndex = ImportDialog.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = ImportDialog.ApplyAllCheck.Checked;
            Properties.Settings.Default.AddChest = AddChestCheck.Checked;
            Properties.Settings.Default.Save();
        }

        private void ExportImageButton_Click(object sender, EventArgs e)
        {
            List<MapIDControl> selected = new List<MapIDControl>();
            foreach (var box in ExistingMaps)
            {
                if (box.Selected)
                    selected.Add(box);
            }
            if (selected.Count > 0)
            {
                ExportDialog.InitialDirectory = LastImgExportPath;
                if (selected.Count == 1)
                    ExportDialog.FileName = selected[0].GetMapName() + ".png";
                else
                    ExportDialog.FileName = "";
                if (ExportDialog.ShowDialog() == DialogResult.OK)
                {
                    LastImgExportPath = Path.GetDirectoryName(ExportDialog.FileName);
                    if (selected.Count == 1)
                        selected[0].Map.Image.Save(ExportDialog.FileName);
                    else
                    {
                        string folder = Path.ChangeExtension(ExportDialog.FileName, "");
                        Directory.CreateDirectory(folder);
                        foreach (var box in selected)
                        {
                            box.Map.Image.Save(Path.Combine(folder, box.GetMapName() + ".png"));
                        }
                    }
                }
            }
        }

        private void AddInventoryButton_Click(object sender, EventArgs e)
        {
            List<long> selected = new List<long>();
            foreach (var box in ExistingMaps)
            {
                if (box.Selected)
                    selected.Add(box.ID);
            }
            if (selected.Count > 0)
                SelectedWorld.AddChestsLocalPlayer(selected);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            List<MapIDControl> selected = new List<MapIDControl>();
            foreach (var box in ExistingMaps)
            {
                if (box.Selected)
                    selected.Add(box);
            }
            if (selected.Count > 0 && MessageBox.Show("Deleting these maps will remove all copies from the world permanently.\n\nWould you like to delete these maps?", $"Delete {selected.Count} maps?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (var box in selected)
                {
                    ExistingMaps.Remove(box);
                    ExistingZone.Controls.Remove(box);
                    SelectedWorld.RemoveMap(box.ID);
                }
            }
        }

        private void TheForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ImportingMaps.Any() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you exit now, these maps will disappear.\n\nWould you like to exit anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
    }

    public enum Edition
    {
        Java,
        Bedrock
    }
}
