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
        if (Version == null)
            throw new InvalidDataException("Could not determine version of world");
        Name = leveldat.RootTag["Data"]?["LevelName"]?.StringValue ?? "";
        WorldIcon = Path.Combine(Folder, "icon.png");
        AccessDate = File.GetLastWriteTime(leveldat.FileName);
    }

    public override bool IsIdTaken(long id)
    {
        return File.Exists(Path.Combine(Folder, "data", $"map_{id}.dat"));
    }

    public override void AddStructures(IEnumerable<StructureGrid> structures, IInventory inventory)
    {
        var items = new List<NbtCompound>();
        foreach (var structure in structures)
        {
            var nbt = Version.CreateStructureFile(structure);
            var path = Version.StructureFileLocation(Folder, structure.Identifier);
            var file = new NbtFile(nbt) { BigEndian = true };
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            file.SaveToFile(path, NbtCompression.GZip);
            var item = Version.MakeStructureItem(structure);
            items.Add(item);
        }
        inventory.AddItems(items);
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
        var full_data = nbt.GetRootTag<NbtCompound>();
        var colors = full_data.Get<NbtCompound>("data").Get<NbtByteArray>("colors").Value;
        var image = Version.Decode(colors);
        return new Map(id, new MapData(image, colors, full_data));
    }

    public override void AddMaps(IEnumerable<Map> maps)
    {
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = true };
            var data = map.Data.FullData;
            if (data == null)
            {
                data = Version.CreateMapCompound(map.Data);
                if (Version.DataVersion != null)
                    data.Add(new NbtInt("DataVersion", Version.DataVersion.Value));
            }
            data.Name = "";
            nbt.RootTag = data;
            var folder = Path.Combine(Folder, "data");
            Directory.CreateDirectory(folder);
            nbt.SaveToFile(Path.Combine(folder, $"map_{map.ID}.dat"), NbtCompression.GZip);
        }
    }

    public override void RemoveMaps(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            var file = Path.Combine(Folder, "data", $"map_{id}.dat");
            if (File.Exists(file))
                File.Delete(file);
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
