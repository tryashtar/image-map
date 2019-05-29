using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace Image_Map
{
    public partial class BedrockWorldWindow : Form
    {
        public bool Confirmed { get; private set; } = false;
        public string SelectedWorldFolder { get; private set; }
        public string SavesFolder { get; set; }
        CommonOpenFileDialog SavesDialog = new CommonOpenFileDialog()
        {
            Title = "Select your Bedrock saves location",
            IsFolderPicker = true,
        };

        public BedrockWorldWindow()
        {
            InitializeComponent();
        }

        private void LoadWorlds(string savesfolder)
        {
            Confirmed = false;
            SelectedWorldFolder = null;
            SavesFolder = savesfolder;
            WorldZone.Controls.Clear();
            foreach (string world in Directory.GetDirectories(savesfolder))
            {
                try
                {
                    var control = new BedrockWorldControl(world);
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
            foreach (BedrockWorldControl control in WorldZone.Controls)
            {
                control.BackColor = Color.Transparent;
            }
            ((BedrockWorldControl)sender).BackColor = Color.LightGreen;
        }

        private void World_DoubleClick(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedWorldFolder = ((BedrockWorldControl)sender).WorldFolder;
            this.Close();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            SavesDialog.InitialDirectory = SavesFolder;
            if (SavesDialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            LoadWorlds(SavesDialog.FileName);
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
}
