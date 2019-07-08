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
    public partial class WorldControl : UserControl
    {
        public string WorldFolder { get; private set; }

        public WorldControl(string worldfolder, Edition edition)
        {
            InitializeComponent();
            WorldFolder = worldfolder;
            string icon;
            string name="";
            if (edition == Edition.Java)
            {
                icon = Path.Combine(worldfolder, "icon.png");
                string leveldat = Path.Combine(worldfolder, "level.dat");
                if (File.Exists(leveldat))
                {
                    try
                    {
                        var nbtfile = new fNbt.NbtFile(leveldat);
                        name = nbtfile.RootTag["Data"]?["LevelName"]?.StringValue;
                    }
                    catch { }
                }
            }
            else
            {
                icon = Path.Combine(worldfolder, "world_icon.jpeg");
                string namepath = Path.Combine(worldfolder, "levelname.txt");
                if (File.Exists(namepath))
                    name = File.ReadAllText(namepath);
            }
            if (File.Exists(icon))
                WorldIcon.Image = Image.FromFile(icon);
            WorldName.Text = name;
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
