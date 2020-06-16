using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;

namespace ImageMap
{
    public partial class TheForm : Form
    {
        private EditionProperties ActiveEdition => EditionProperties.FromEdition(OpenedWorld.Edition);
        private MinecraftWorld OpenedWorld;
        public TheForm()
        {
            InitializeComponent();
        }

        private void TheForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void JavaWorldButton_Click(object sender, EventArgs e)
        {
            BrowseForWorld(JavaEditionProperties.Instance);
        }

        private void BedrockWorldButton_Click(object sender, EventArgs e)
        {
            BrowseForWorld(BedrockEditionProperties.Instance);
        }

        private void OpenWorld(MinecraftWorld world)
        {
            if (!WorldView.HasUnsavedChanges() || MessageBox.Show("You have unsaved maps waiting to be imported! If you select a new world, these will be lost!\n\nDiscard unsaved maps?", "Wait a minute!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OpenedWorld = world;
                WorldView.SetWorld(OpenedWorld);
            }
        }

        private void BrowseForWorld(EditionProperties edition)
        {
            if (String.IsNullOrEmpty(edition.SavesFolder) || !Directory.Exists(edition.SavesFolder))
                edition.SavesFolder = edition.DefaultSavesFolder();
            edition.BrowseDialog.SavesFolder = edition.SavesFolder;
            edition.BrowseDialog.Show(this);
            if (!edition.BrowseDialog.Confirmed)
                return;
            edition.SavesFolder = edition.BrowseDialog.SavesFolder;
            var world = edition.OpenWorld(edition.BrowseDialog.SavesFolder);
            OpenWorld(world);
        }

        // drag and drop worlds
        private void TheForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void TheForm_DragDrop(object sender, DragEventArgs e)
        {
            string file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
            DraggedWorld(file);
        }

        private void DraggedWorld(string folder)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    OpenedWorld = EditionProperties.AutoOpenWorld(folder);
                    WorldView.SetWorld(OpenedWorld);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Couldn't tell what edition of Minecraft that world was.", "Not a world?");
                }
            }
            else
                MessageBox.Show("Only world folders can be opened. Please don't drag ZIPs or MCWORLDs.\nIf you were trying to drag images, make sure to open a world first.", "Not a folder?");
        }

        private void TheForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorldView.HasUnsavedChanges() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you exit now, these maps will disappear.\n\nWould you like to exit anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (MapViewZone.Visible)
            //{
            //    if (MapView.SelectedTab == ImportTab)
            //    {
            //        if (keyData == (Keys.V | Keys.Control))
            //        {
            //            if (Clipboard.ContainsFileDropList())
            //            {
            //                var files = Clipboard.GetFileDropList();
            //                string[] array = new string[files.Count];
            //                files.CopyTo(array, 0);
            //                Controller.ImportImages(array);
            //            }
            //            else if (Clipboard.ContainsImage())
            //            {
            //                var image = Clipboard.GetImage();
            //                Controller.ImportImages(image);
            //            }
            //            return true;
            //        }
            //        else if (keyData == Keys.Delete)
            //        {
            //            ImportContextDiscard_Click(this, new EventArgs());
            //            return true;
            //        }
            //    }
            //    else if (MapView.SelectedTab == ExistingTab)
            //    {
            //        if (keyData == Keys.Delete)
            //        {
            //            ExistingContextDelete_Click(this, new EventArgs());
            //            return true;
            //        }
            //    }
            //}
            //if (keyData == (Keys.A | Keys.Control))
            //{
            //    if (MapView.SelectedTab == ImportTab)
            //        Controller.SelectAll(MapStatus.Importing);
            //    else if (MapView.SelectedTab == ExistingTab)
            //        Controller.SelectAll(MapStatus.Existing);
            //    return true;
            //}
            //else if (keyData == (Keys.D | Keys.Control))
            //{
            //    if (MapView.SelectedTab == ImportTab)
            //        Controller.DeselectAll(MapStatus.Importing);
            //    else if (MapView.SelectedTab == ExistingTab)
            //        Controller.DeselectAll(MapStatus.Existing);
            //    return true;
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
