using fNbt;
using LevelDBWrapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

    public abstract IEnumerable<Map> GetMaps();
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

    public override IEnumerable<Map> GetMaps()
    {
        var maps = Path.Combine(Folder, "data");
        if (Directory.Exists(maps))
        {
            foreach (var file in Directory.EnumerateFiles(maps, "*.dat"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name.StartsWith("map_"))
                {
                    long id = long.Parse(name[4..]);
                    yield return new Map(id, null);
                }
            }
        }
    }
}

public class BedrockWorld : World
{
    public BedrockWorld(string folder) : base(folder)
    {
        string file = Path.Combine(folder, "levelname.txt");
        if (File.Exists(file))
            Name = File.ReadLines(file).FirstOrDefault();
        WorldIcon = Path.Combine(Folder, "world_icon.jpeg");
    }

    public override IEnumerable<Map> GetMaps()
    {
        using var db = OpenDB();
        using var iterator = db.CreateIterator();
        const string MapKeyword = "map";
        iterator.Seek(MapKeyword);
        while (iterator.IsValid())
        {
            var name = iterator.StringKey();
            if (name.StartsWith(MapKeyword))
            {
                long id = long.Parse(name[4..]);
                var bytes = iterator.Value();
                var nbt = new NbtFile() { BigEndian = false };
                nbt.LoadFromBuffer(bytes, 0, bytes.Length, NbtCompression.None);
                var image = Image.LoadPixelData<Rgba32>(nbt.RootTag.Get<NbtByteArray>("colors").Value, 128, 128);
                yield return new Map(id, new ImageSharpImageSource<Rgba32>(image));
            }
            else
                break;
            iterator.Next();
        }
    }

    private LevelDB OpenDB()
    {
        return new LevelDB(Path.Combine(Folder, "db"));
    }
}

