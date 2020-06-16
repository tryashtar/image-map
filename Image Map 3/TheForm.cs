using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;


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

        private void BrowseForWorld(EditionProperties edition)
        {
            if (String.IsNullOrEmpty(edition.SavesFolder) || !Directory.Exists(edition.SavesFolder))
                edition.SavesFolder = edition.DefaultSavesFolder();
            edition.BrowseDialog.SavesFolder = edition.SavesFolder;
            edition.BrowseDialog.Show(this);
            if (!edition.BrowseDialog.Confirmed)
                return;
            edition.SavesFolder = edition.BrowseDialog.SavesFolder;
            OpenedWorld = edition.OpenWorld(edition.BrowseDialog.SavesFolder);
            //WorldView.SetWorld(OpenedWorld);
        }

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

        private void TheForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (WorldView.HasUnsavedChanges() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you exit now, these maps will disappear.\n\nWould you like to exit anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
            //    e.Cancel = true;
        }

        private void SendMapsWithMessage(IEnumerable<MapIDControl> maps, string destination)
        {
            //int conflicts = Controller.SendMapsToWorld(maps, MapReplaceOption.Info, destination);
            //if (conflicts > 0)
            //{
            //    var option = new ReplaceOptionDialog(conflicts);
            //    option.ShowDialog(this);
            //    Controller.SendMapsToWorld(maps, option.SelectedOption, destination);
            //}
            //else
            //    Controller.SendMapsToWorld(maps, MapReplaceOption.ReplaceExisting, destination);
        }

        private void OpenWorld(Edition edition, string folder)
        {
            //var result = Controller.OpenWorld(edition, folder);
            //if (result == ActionResult.MapsNotImported && MessageBox.Show("You have unsaved maps waiting to be imported! If you select a new world, these will be lost!\n\nDiscard unsaved maps?", "Wait a minute!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    Controller.OpenWorld(edition, folder, bypass_mapwarning: true);
        }

        private void DraggedWorld(string folder)
        {
            if (!Directory.Exists(folder))
            {
                MessageBox.Show("Only world folders can be opened. Please don't drag ZIPs or MCWORLDs.\nIf you were trying to drag images, make sure to open a world first.", "Not a folder?");
                return;
            }
            if (File.Exists(Path.Combine(folder, "db", "CURRENT")))
                OpenWorld(Edition.Bedrock, folder);
            else if (Directory.Exists(Path.Combine(folder, "region")))
                OpenWorld(Edition.Java, folder);
            else
                MessageBox.Show("Couldn't tell what edition of Minecraft that world was.", "Not a world?");
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

        private void ChangeMapIDs(IEnumerable<MapIDControl> boxes, MapStatus area)
        {
            //var input = new IDInputDialog(boxes.First().ID);
            //input.ShowDialog(this);
            //if (input.Confirmed)
            //{
            //    long firstid;
            //    if (input.WantsAuto)
            //        firstid = Controller.GetSafeID();
            //    else
            //        firstid = input.SelectedID;
            //    int count = Controller.ChangeMapIDs(boxes, firstid, area, MapReplaceOption.Info);
            //    if (count > 0)
            //    {
            //        var picker = new ReplaceOptionDialog(count);
            //        picker.ShowDialog(this);
            //        Controller.ChangeMapIDs(boxes, firstid, area, picker.SelectedOption);
            //    }
            //    else
            //        Controller.ChangeMapIDs(boxes, firstid, area, MapReplaceOption.Skip);
            //}
        }
    }
}
