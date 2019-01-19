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
        ViewController Controller;
        static readonly string[] ImageExtensions = new[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif" };
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
            Multiselect = true,
        };
        BedrockWorldWindow WorldDialog = new BedrockWorldWindow();
        public TheForm()
        {
            InitializeComponent();
            Controller = new ViewController(this);
            OpenDialog.Filter = GenerateFilter("Image Files", ImageExtensions);
        }

        private static string GenerateFilter(string description, string[] extensions)
        {
            string result = description + "|";
            foreach (string extension in extensions)
            {
                result += "*" + extension + ";";
            }
            result += "|All Files|*.*";
            return result;
        }

        private void TheForm_Load(object sender, EventArgs e)
        {
            // load up saved settings
            LastOpenPath = Properties.Settings.Default.LastOpenPath;
            LastWorldPath = Properties.Settings.Default.LastWorldPath;
            LastImgExportPath = Properties.Settings.Default.LastImgExportPath;
            AddChestCheck.Checked = Properties.Settings.Default.AddNewMaps;
        }

        private void OpenWorldWithMessage(Edition edition)
        {
            string folder = null;
            // edition-specific world picking
            if (edition == Edition.Java)
            {
                SelectWorldDialog.InitialDirectory = LastWorldPath;
                if (SelectWorldDialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return;
                folder = SelectWorldDialog.FileName;
            }
            else if (edition == Edition.Bedrock)
            {
                if (!Directory.Exists(BedrockSavesFolder))
                {
                    MessageBox.Show("You seemingly don't have Bedrock Edition installed.", "Hmm...");
                    return;
                }
                WorldDialog.ShowWorlds(this, BedrockSavesFolder);
                if (!WorldDialog.Confirmed)
                    return;
                folder = WorldDialog.SelectedWorldFolder;
            }
            // generic world opening and warning
            var result = Controller.OpenWorld(edition, folder);
            if (result == ActionResult.MapsNotImported && MessageBox.Show("You have unsaved maps waiting to be imported! If you select a new world, these will be lost!\n\nDiscard unsaved maps?", "Wait a minute!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                result = Controller.OpenWorld(edition, folder, bypass_mapwarning: true);
        }

        private void JavaWorldButton_Click(object sender, EventArgs e)
        {
            OpenWorldWithMessage(Edition.Java);
        }


        private void BedrockWorldButton_Click(object sender, EventArgs e)
        {
            OpenWorldWithMessage(Edition.Bedrock);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenDialog.InitialDirectory = LastOpenPath;
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                LastOpenPath = Path.GetDirectoryName(OpenDialog.FileName);
                Controller.ImportImages(OpenDialog.FileNames);
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (AddChestCheck.Checked)
                Controller.SendMapsToWorld(PlayerSelector.Text);
            else
                Controller.SendMapsToWorld();
        }

        private void TheForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save settings
            Properties.Settings.Default.LastWorldPath = LastWorldPath;
            Properties.Settings.Default.LastOpenPath = LastOpenPath;
            Properties.Settings.Default.LastImgExportPath = LastImgExportPath;
            Properties.Settings.Default.AddNewMaps = AddChestCheck.Checked;
            Properties.Settings.Default.Save();
        }

        private void ExportImageButton_Click(object sender, EventArgs e)
        {
            var selected = Controller.GetSelectedMaps();
            // super epic way to check if there is exactly one item
            bool onlyone = selected.Take(2).Count() == 1;
            if (onlyone)
                ExportDialog.FileName = selected.First().GetMapName() + ".png";
            else
                ExportDialog.FileName = "";
            ExportDialog.InitialDirectory = LastImgExportPath;
            if (ExportDialog.ShowDialog() == DialogResult.OK)
            {
                LastImgExportPath = Path.GetDirectoryName(ExportDialog.FileName);
                if (onlyone)
                    Controller.SaveMap(selected.First(), ExportDialog.FileName);
                else
                    Controller.SaveMaps(selected, Path.ChangeExtension(ExportDialog.FileName, ""));
            }
        }

        private void AddInventoryButton_Click(object sender, EventArgs e)
        {
            bool success = Controller.AddChests(Controller.GetSelectedMaps().Select(x => x.ID), PlayerSelector.Text);
            if (!success)
                MessageBox.Show("There wasn't enough space to fit the chests in your inventory. One or more were not added.", "Chest alert!");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var selected = Controller.GetSelectedMaps();
            if (selected.Any() && MessageBox.Show("Deleting these maps will remove all copies from the world permanently.\n\nWould you like to delete these maps?", $"Delete {selected.Count()} maps?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Controller.DeleteMapsFromWorld(selected);
        }

        private void TheForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Controller.UnsavedChanges() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you exit now, these maps will disappear.\n\nWould you like to exit anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }

        private void PlayerSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.GiveChest = (PlayerSelector.SelectedIndex != 1);
        }

        private void ImportZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportZone_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Controller.ImportImages(files);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (MapView.Visible && MapView.SelectedTab == ImportTab)
            {
                if (keyData == (Keys.V | Keys.Control))
                {
                    if (Clipboard.ContainsFileDropList())
                    {
                        var files = Clipboard.GetFileDropList();
                        string[] array = new string[files.Count];
                        files.CopyTo(array, 0);
                        Controller.ImportImages(array);
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
