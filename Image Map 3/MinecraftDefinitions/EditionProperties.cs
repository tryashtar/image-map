using System;
using System.Collections.Generic;
using System.IO;

namespace ImageMap
{
    public enum Edition
    {
        Java,
        Bedrock
    }

    public abstract class EditionProperties
    {
        public abstract string SavesFolder { get; set; }
        public abstract WorldSelectWindow BrowseDialog { get; }
        public abstract string DefaultSavesFolder();
        public abstract ImportWindow CreateImportWindow();
        public abstract MinecraftWorld OpenWorld(string folder);

        public static MinecraftWorld AutoOpenWorld(string folder)
        {
            if (Directory.Exists(Path.Combine(folder, "region")))
                return JavaEditionProperties.Instance.OpenWorld(folder);
            if (File.Exists(Path.Combine(folder, "db", "CURRENT")))
                return BedrockEditionProperties.Instance.OpenWorld(folder);
            throw new FileNotFoundException("Could not determine what type of world this is");
        }

        private static readonly Dictionary<Edition, EditionProperties> InstanceMap = new Dictionary<Edition, EditionProperties>();
        public Edition Edition { get; private set; }
        protected EditionProperties(Edition edition)
        {
            Edition = edition;
            InstanceMap[edition] = this;
        }
        public static EditionProperties FromEdition(Edition edition) => InstanceMap[edition];
    }
}
