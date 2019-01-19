using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public partial class BedrockWorldWindow : Form
    {
        public bool Confirmed { get; private set; } = false;
        public string SelectedWorldFolder { get; private set; }


        public BedrockWorldWindow()
        {
            InitializeComponent();
        }

        public void ShowWorlds(Form parent, string folder)
        {
            Confirmed = false;
            SelectedWorldFolder = null;
            WorldZone.Controls.Clear();
            foreach (string world in Directory.GetDirectories(folder))
            {
                try
                {
                    var control = new BedrockWorldControl(world);
                    WorldZone.Controls.Add(control);
                    control.Click += World_Click;
                    control.DoubleClick += World_DoubleClick;
                }
                catch (IOException) { } // if it couldn't get the world files it needed 
            }
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
    }
}
