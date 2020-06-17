using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace ImageMap
{
    public abstract partial class WorldSelectWindow : Form
    {
        public bool Confirmed { get; private set; } = false;
        public string SelectedWorldFolder { get; private set; }
        public string SavesFolder { get; set; }

        public abstract WorldIconControl NewWorldControl(string folder);
        protected abstract string GetTitle();

        public WorldSelectWindow()
        {
            InitializeComponent();
            this.Text = GetTitle();
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
                    var control = NewWorldControl(world);
                    WorldZone.Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                    control.Click += World_Click;
                    control.DoubleClick += World_DoubleClick;
                }
                catch (IOException ex)
                {
                    // if it couldn't get the world files it needed
                    MessageBox.Show($"There was an error loading worlds:\n\n{Util.ExceptionMessage(ex)}", "Error loading worlds");
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
            foreach (WorldIconControl control in WorldZone.Controls)
            {
                control.BackColor = Color.Transparent;
            }
            ((WorldIconControl)sender).BackColor = Color.LightGreen;
        }

        private void World_DoubleClick(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedWorldFolder = ((WorldIconControl)sender).WorldFolder;
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

    public class JavaWorldWindow : WorldSelectWindow
    {
        public override WorldIconControl NewWorldControl(string folder)
        {
            return new JavaWorldControl(folder);
        }
        protected override string GetTitle() => "Java Worlds";
    }

    public class BedrockWorldWindow : WorldSelectWindow
    {
        public override WorldIconControl NewWorldControl(string folder)
        {
            return new BedrockWorldControl(folder);
        }
        protected override string GetTitle() => "Bedrock Worlds";
    }
}
