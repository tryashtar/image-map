using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace ImageMap
{
    public partial class WorldWindow : Form
    {
        public bool Confirmed { get; private set; } = false;
        public string SelectedWorldFolder { get; private set; }
        public string SavesFolder { get; set; }
        public Edition Edition { get; private set; }

        public WorldWindow(Edition edition)
        {
            InitializeComponent();
            Edition = edition;
        }

        private void LoadWorlds(string savesfolder)
        {
            Confirmed = false;
            SelectedWorldFolder = null;
            SavesFolder = savesfolder;
            WorldZone.Controls.Clear();
            if (!Directory.Exists(savesfolder))
                savesfolder = Directory.GetCurrentDirectory();
            foreach (string world in Directory.GetDirectories(savesfolder).OrderByDescending(x => Directory.GetLastWriteTime(x)))
            {
                try
                {
                    var control = new WorldControl(world, Edition);
                    control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    WorldZone.Controls.Add(control);
                    control.Click += World_Click;
                    control.DoubleClick += World_DoubleClick;
                }
                catch (IOException ex)
                {
                    // if it couldn't get the world files it needed
                    MessageBox.Show("There was an error loading worlds: " + ex.Message, "Error loading worlds");
                }
            }
        }

        public void Show(Form parent)
        {
            LoadWorlds(SavesFolder);
            ShowDialog(parent);
        }

        private void World_Click(object sender, EventArgs e)
        {
            foreach (WorldControl control in WorldZone.Controls)
            {
                control.BackColor = Color.Transparent;
            }
            ((WorldControl)sender).BackColor = Color.LightGreen;
        }

        private void World_DoubleClick(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedWorldFolder = ((WorldControl)sender).WorldFolder;
            this.Close();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var picker = new FolderPicker("Select your world saves location", SavesFolder);
            if (picker.ShowDialog() == DialogResult.OK)
                LoadWorlds(picker.SelectedFolder);
        }

        // press ESC to close window
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }

    // fall back to crappy folder picker if unsupported
    public class FolderPicker
    {
        public readonly string Title;
        public readonly string InitialFolder;
        public string SelectedFolder { get; private set; } = null;
        public FolderPicker(string title, string initial_folder)
        {
            Title = title;
            InitialFolder = initial_folder;
        }

        public DialogResult ShowDialog()
        {
            var good_browser = new CommonOpenFileDialog()
            {
                Title = this.Title,
                InitialDirectory = this.InitialFolder,
                IsFolderPicker = true
            };
            try
            {
                var result = good_browser.ShowDialog() == CommonFileDialogResult.Ok ? DialogResult.OK : DialogResult.Cancel;
                if (result == DialogResult.OK)
                    SelectedFolder = good_browser.FileName;
                return result;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                var crappy_browser = new FolderBrowserDialog()
                {
                    Description = this.Title,
                    SelectedPath = this.InitialFolder
                };
                var result = crappy_browser.ShowDialog();
                if (result == DialogResult.OK)
                    SelectedFolder = crappy_browser.SelectedPath;
                return result;
            }
        }
    }
}
