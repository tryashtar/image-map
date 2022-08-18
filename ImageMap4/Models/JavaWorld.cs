using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;

namespace ImageMap4;

public class JavaWorld : World
{
    public IJavaVersion Version { get; }
    public override string Name { get; }
    public override string WorldIcon { get; }
    public override DateTime AccessDate { get; }

    public JavaWorld(string folder) : base(folder)
    {
        var leveldat = new NbtFile(Path.Combine(Folder, "level.dat"));
        Version = VersionManager.DetermineJavaVersion(leveldat.GetRootTag<NbtCompound>().Get<NbtCompound>("Data"));
        Name = leveldat.RootTag["Data"]?["LevelName"]?.StringValue ?? "";
        WorldIcon = Path.Combine(Folder, "icon.png");
        AccessDate = File.GetLastWriteTime(leveldat.FileName);
    }

    public override void AddStructure(StructureGrid structure, IInventory inventory)
    {
        var nbt = Version.CreateStructureFile(structure);
        var path = Version.StructureFileLocation(Folder, structure.Identifier);
        var file = new NbtFile(nbt) { BigEndian = true };
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        file.SaveToFile(path, NbtCompression.GZip);
        var item = Version.MakeStructureItem(structure);
        inventory.AddItem(item);
    }

    public override IEnumerable<IInventory> GetInventories()
    {
        yield return new JavaInventory("Local player", Path.Combine(Folder, "level.dat"), NbtPath.Parse("Data.Player.Inventory"));
        var playerdata = Path.Combine(Folder, "playerdata");
        if (Directory.Exists(playerdata))
        {
            foreach (var file in Directory.GetFiles(playerdata, "*.dat"))
            {
                string uuid = Path.GetFileNameWithoutExtension(file);
                if (uuid.Length == 36)
                    yield return new JavaInventory(uuid, file, NbtPath.Parse("Inventory"));
            }
        }
    }

    public override async IAsyncEnumerable<Map> GetMapsAsync()
    {
        var maps = Path.Combine(Folder, "data");
        if (Directory.Exists(maps))
        {
            foreach (var file in Directory.EnumerateFiles(maps, "*.dat"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name.StartsWith("map_"))
                    yield return await Task.Run(() => GetMap(file));
            }
        }
    }

    private Map GetMap(string file)
    {
        string name = Path.GetFileNameWithoutExtension(file);
        long id = long.Parse(name[4..]);
        var nbt = new NbtFile() { BigEndian = true };
        nbt.LoadFromFile(file, NbtCompression.GZip, null);
        var colors = nbt.GetRootTag<NbtCompound>().Get<NbtCompound>("data").Get<NbtByteArray>("colors").Value;
        var image = Version.Decode(colors);
        ProcessImage(image, new ProcessSettings(null, new EuclideanAlgorithm()));
        return new Map(id, new MapData(image, colors));
    }

    public override void AddMaps(IEnumerable<Map> maps)
    {
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = true };
            var data = Version.CreateMapCompound(map.Data);
            data.Name = "data";
            nbt.GetRootTag<NbtCompound>().Add(data);
            nbt.SaveToFile(Path.Combine(Folder, "data", $"map_{map.ID}.dat"), NbtCompression.GZip);
        }
    }

    protected override void ProcessImage(Image<Rgba32> image, ProcessSettings settings)
    {
        var palette = Version.GetPalette();
        var quantizer = new CustomQuantizer(new QuantizerOptions() { Dither = settings.Dither }, palette, settings.Algorithm);
        image.Mutate(x => x.Quantize(quantizer));
    }

    protected override byte[] EncodeColors(Image<Rgba32> image) => Version.EncodeColors(image);
}
