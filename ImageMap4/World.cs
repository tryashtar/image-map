using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;

public abstract class World
{
    public string Folder { get; }
    public string FolderName => Path.GetFileName(Folder);
    public string Name { get; protected set; }
    public string WorldIcon { get; protected set; }
    public World(string folder)
    {
        Folder = folder;
    }
}

public class JavaWorld : World
{
    public readonly NbtFile LevelDat;
    public JavaWorld(string folder) : base(folder)
    {
        LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
        Name = LevelDat.RootTag["Data"]?["LevelName"]?.StringValue;
        WorldIcon = Path.Combine(Folder, "icon.png");
    }
}

public class BedrockWorld : World
{
    public BedrockWorld(string folder) : base(folder)
    {
        string file = Path.Combine(folder, "levelname.txt");
        if (File.Exists(file))
            Name = File.ReadAllText(file);
        WorldIcon = Path.Combine(Folder, "world_icon.jpeg");
    }
}

