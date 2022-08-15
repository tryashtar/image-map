using fNbt;
using LevelDBWrapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMap4;

public class BedrockWorld : World
{
    public IBedrockVersion Version { get; }
    public BedrockWorld(string folder) : base(folder)
    {
        string file = Path.Combine(folder, "levelname.txt");
        if (File.Exists(file))
            Name = File.ReadLines(file).FirstOrDefault();
        WorldIcon = Path.Combine(Folder, "world_icon.jpeg");
        using var leveldat = File.OpenRead(Path.Combine(folder, "level.dat"));
        leveldat.Position = 8;
        var nbt = new NbtFile() { BigEndian = false };
        nbt.LoadFromStream(leveldat, NbtCompression.None);
        Version = DetermineVersionFromLevelDat(nbt.RootTag);
        AccessDate = File.GetLastWriteTime(leveldat.Name);
    }

    private static IBedrockVersion DetermineVersionFromLevelDat(NbtCompound leveldat)
    {
        var versiontag = leveldat["lastOpenedWithVersion"];
        if (versiontag is NbtList list)
        {
            var minor = list[1];
            if (minor is NbtInt num)
            {
                if (num.Value >= 11)
                    return new Bedrock1p11Version();
                if (num.Value >= 7)
                    return new Bedrock1p7Version();
                if (num.Value >= 2)
                    return new Bedrock1p2Version();
            }
        }
        throw new InvalidOperationException("Couldn't determine world version");
    }

    public override void AddStructure(StructureGrid structure, Inventory inventory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Inventory> GetInventories()
    {
        yield return new LocalInventory();
    }

    private class LocalInventory : Inventory
    {
        public override string Name => "Local player";
        public override void AddItem(NbtCompound item)
        {
            throw new NotImplementedException();
        }
    }

    public override async IAsyncEnumerable<Map> GetMapsAsync()
    {
        using var db = OpenDB();
        using var iterator = db.CreateIterator();
        iterator.Seek("map_");
        while (iterator.IsValid())
        {
            var name = iterator.StringKey();
            if (name.StartsWith("map_"))
                yield return await Task.Run(() => GetMap(iterator));
            else
                break;
            iterator.Next();
        }
    }

    private Map GetMap(Iterator iterator)
    {
        var name = iterator.StringKey();
        long id = long.Parse(name[4..]);
        var bytes = iterator.Value();
        var nbt = new NbtFile() { BigEndian = false };
        nbt.LoadFromBuffer(bytes, 0, bytes.Length, NbtCompression.None);
        var colors = nbt.RootTag.Get<NbtByteArray>("colors").Value;
        var image = Image.LoadPixelData<Rgba32>(colors, 128, 128);
        return new Map(id, new MapData(image, colors));
    }

    public override void AddMaps(IEnumerable<Map> maps)
    {
        using var db = OpenDB();
        using var batch = new WriteBatch();
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = false };
            nbt.RootTag = Version.CreateMapCompound(map);
            var bytes = nbt.SaveToBuffer(NbtCompression.None);
            batch.Put($"map_{map.ID}", bytes);
        }
        db.Write(batch);
    }

    protected override void ProcessImage(Image<Rgba32> image, ProcessSettings settings)
    {
        // no quantization needed, Bedrock supports all colors
    }

    protected override byte[] EncodeColors(Image<Rgba32> image)
    {
        var result = new byte[128 * 128 * 4];
        image.CopyPixelDataTo(result);
        return result;
    }

    private LevelDB OpenDB()
    {
        return new LevelDB(Path.Combine(Folder, "db"));
    }
}
