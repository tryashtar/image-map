using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Image_Map
{
    public partial class BedrockWorldControl : UserControl
    {
        public string WorldFolder { get; private set; }

        public BedrockWorldControl(string worldfolder)
        {
            InitializeComponent();
            WorldFolder = worldfolder;
            string icon = Path.Combine(worldfolder, "world_icon.jpeg");
            if (File.Exists(icon))
                WorldIcon.Image = Image.FromFile(icon);
            string namepath = Path.Combine(worldfolder, "levelname.txt");
            if (File.Exists(namepath))
                WorldName.Text = File.ReadAllText(namepath);
            FolderName.Text = Path.GetFileName(worldfolder);
        }

        private void Control_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void Control_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }
    }
}
