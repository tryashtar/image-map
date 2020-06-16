using System;
using System.Collections.Generic;
using System.IO;

namespace ImageMap
{
    public abstract class EditionProperties
    {
        public abstract string SavesFolder { get; set; }
        public abstract WorldWindow BrowseDialog { get; }
        public abstract string DefaultSavesFolder();
        public abstract ImportWindow CreateImportWindow();
        public abstract MinecraftWorld OpenWorld(string folder);

        private static readonly Dictionary<Edition, EditionProperties> InstanceMap = new Dictionary<Edition, EditionProperties>();
        public Edition Edition { get; private set; }
        protected EditionProperties(Edition edition)
        {
            Edition = edition;
            InstanceMap[edition] = this;
        }
        public static EditionProperties FromEdition(Edition edition) => InstanceMap[edition];
    }

    public class JavaEditionProperties : EditionProperties
    {
        public static JavaEditionProperties Instance = new JavaEditionProperties();
        private readonly WorldWindow Dialog;
        public override WorldWindow BrowseDialog => Dialog;
        private JavaEditionProperties() : base(Edition.Java)
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
        public override ImportWindow CreateImportWindow()
        {
            return new ImportWindow(true);
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
        public static BedrockEditionProperties Instance = new BedrockEditionProperties();
        private readonly WorldWindow Dialog;
        public override WorldWindow BrowseDialog => Dialog;
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
            world.Initialize();
            return world;
        }
    }
}
