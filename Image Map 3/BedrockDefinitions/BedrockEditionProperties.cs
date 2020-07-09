using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public class BedrockEditionProperties : EditionProperties
    {
        public static BedrockEditionProperties Instance = new BedrockEditionProperties();
        private readonly WorldSelectWindow Dialog;
        public override WorldSelectWindow BrowseDialog => Dialog;
        private BedrockEditionProperties() : base(Edition.Bedrock)
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
        public override ImportWindow CreateImportWindow()
        {
            return new ImportWindow(false);
        }
        public override MinecraftWorld OpenWorld(string folder)
        {
            var world = new BedrockWorld(folder);
            world.LoadMapsFront(50);
            world.LoadMapsBack(50);
            return world;
        }
    }
}
