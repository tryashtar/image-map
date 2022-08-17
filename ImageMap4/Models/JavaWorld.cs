using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageMap4;

public class JavaWorld : World
{
    public readonly NbtFile LevelDat;
    public IJavaVersion Version { get; }
    public override string Name { get; }
    public override string WorldIcon { get; }
    public override DateTime AccessDate { get; }

    public JavaWorld(string folder) : base(folder)
    {
        LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
        Version = VersionManager.DetermineVersion(LevelDat.GetRootTag<NbtCompound>().Get<NbtCompound>("Data"));
        Name = LevelDat.RootTag["Data"]?["LevelName"]?.StringValue ?? "";
        WorldIcon = Path.Combine(Folder, "icon.png");
        AccessDate = File.GetLastWriteTime(LevelDat.FileName);
    }

    public override void AddStructure(StructureGrid structure, Inventory? inventory)
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

    private class UUIDInventory : Inventory
    {
        private string _name;
        public override string Name => _name;
        private string File;
        public UUIDInventory(string file)
        {
            File = file;
            _name = Path.GetFileNameWithoutExtension(file);
        }
        public override void AddItem(NbtCompound item)
        {
            throw new NotImplementedException();
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
