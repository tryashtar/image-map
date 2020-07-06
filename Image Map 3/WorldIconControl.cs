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

namespace ImageMap
{
    public abstract partial class WorldIconControl : UserControl
    {
        public string WorldFolder { get; private set; }
        protected abstract string GetIconPath(string folder);
        protected abstract string GetWorldName(string folder);

        public WorldIconControl(string worldfolder)
        {
            InitializeComponent();
            WorldFolder = worldfolder;
            string icon = GetIconPath(worldfolder);
            string name = GetWorldName(worldfolder);
            if (File.Exists(icon))
            {
                try
                { WorldIcon.Image = Image.FromFile(icon); }
                catch
                { WorldIcon.Image = Properties.Resources.image_map_icon; }
            }
            else
                WorldIcon.Image = Properties.Resources.image_map_icon;
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

    public class JavaWorldControl : WorldIconControl
    {
        public JavaWorldControl(string folder) : base(folder) { }

        protected override string GetIconPath(string folder)
        {
            return Path.Combine(folder, "icon.png");
        }

        protected override string GetWorldName(string folder)
        {
            string leveldat = Path.Combine(folder, "level.dat");
            if (File.Exists(leveldat))
            {
                try
                {
                    var nbtfile = new fNbt.NbtFile(leveldat);
                    return nbtfile.RootTag["Data"]?["LevelName"]?.StringValue;
                }
                catch
                { return null; }
            }
            return null;
        }
    }

    public class BedrockWorldControl : WorldIconControl
    {
        public BedrockWorldControl(string folder) : base(folder) { }

        protected override string GetIconPath(string folder)
        {
            return Path.Combine(folder, "world_icon.jpeg");
        }

        protected override string GetWorldName(string folder)
        {
            string namepath = Path.Combine(folder, "levelname.txt");
            if (File.Exists(namepath))
                return File.ReadAllText(namepath);
            return null;
        }
    }
}
