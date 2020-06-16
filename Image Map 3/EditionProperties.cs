using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public abstract class EditionProperties
    {
        public abstract string SavesFolder { get; set; }
        public abstract WorldWindow BrowseDialog { get; }
        public abstract string DefaultSavesFolder();
        public abstract MinecraftWorld OpenWorld(string folder);
    }

    public class JavaEditionProperties : EditionProperties
    {
        private readonly WorldWindow Dialog;
        public override WorldWindow BrowseDialog => Dialog;
        public JavaEditionProperties()
        {
            Dialog = new JavaWorldWindow();
        }
        public override string SavesFolder
        {
            get => Properties.Settings.Default.JavaSavesFolder;
            set { Properties.Settings.Default.JavaSavesFolder = value; }
        }
        public override string DefaultSavesFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @".minecraft\saves");
        }
        public override MinecraftWorld OpenWorld(string folder)
        {
            var world = new JavaWorld(folder);
            world.Initialize();
            return world;
        }
    }

    public class BedrockEditionProperties : EditionProperties
    {
        private readonly WorldWindow Dialog;
        public override WorldWindow BrowseDialog => Dialog;
        public BedrockEditionProperties()
        {
            Dialog = new BedrockWorldWindow();
        }
        public override string SavesFolder
        {
            get => Properties.Settings.Default.BedrockSavesFolder;
            set { Properties.Settings.Default.BedrockSavesFolder = value; }
        }
        public override string DefaultSavesFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
        }
        public override MinecraftWorld OpenWorld(string folder)
        {
            var world = new BedrockWorld(folder);
            world.Initialize();
            return world;
        }
    }
}
