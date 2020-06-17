using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace ImageMap
{
    public partial class TheForm : Form
    {
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

        // use a function to defer loading of the world; it's pointless to construct a world only to discard it if the user wants to wait
        private void OpenWorld(Func<MinecraftWorld> getworld)
        {
            if (!WorldView.HasUnsavedChanges() || MessageBox.Show("You have unsaved maps waiting to be imported! If you select a new world, these will be lost!\n\nDiscard unsaved maps?", "Wait a minute!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MinecraftWorld world = null;
                try
                {
                    world = getworld();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening that world:\n\n{Util.ExceptionMessage(ex)}", "World error!");
                }
                if (world != null)
                {
                    OpenedWorld = world;
                    WorldView.SetWorld(OpenedWorld);
                    MapViewZone.Visible = true;
                    this.Text = "Image Map – " + OpenedWorld.Name;
                }
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
            OpenWorld(() => edition.OpenWorld(edition.BrowseDialog.SelectedWorldFolder));
        }

        private void DraggedWorld(string folder)
        {
            if (Directory.Exists(folder))
            {
                OpenWorld(() => EditionProperties.AutoOpenWorld(folder));
            }
            else
                MessageBox.Show("Only world folders can be opened. Please don't drag ZIPs or MCWORLDs.\nIf you were trying to drag images, make sure to open a world first.", "Not a folder?");
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

        private void TheForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorldView.HasUnsavedChanges() && MessageBox.Show("You imported some maps, but you haven't sent them over to the world yet. You need to press \"Send All to World\" to do that. If you exit now, these maps will disappear.\n\nWould you like to exit anyway?", "Wait a Minute!", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
