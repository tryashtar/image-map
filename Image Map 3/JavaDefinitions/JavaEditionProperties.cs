using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public class JavaEditionProperties : EditionProperties
    {
        public static JavaEditionProperties Instance = new JavaEditionProperties();
        private readonly WorldSelectWindow Dialog;
        public override WorldSelectWindow BrowseDialog => Dialog;
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
            world.LoadMapsFront(25);
            world.LoadMapsBack(25);
            return world;
        }
    }
}
