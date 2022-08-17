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
        Version = DetermineVersionFromLevelDat(LevelDat.GetRootTag<NbtCompound>().Get<NbtCompound>("Data"));
        Name = LevelDat.RootTag["Data"]?["LevelName"]?.StringValue ?? "";
        WorldIcon = Path.Combine(Folder, "icon.png");
        AccessDate = File.GetLastWriteTime(LevelDat.FileName);
    }

    private static IJavaVersion DetermineVersionFromLevelDat(NbtCompound leveldat)
    {
        var dataversion = leveldat["DataVersion"];
        if (dataversion is NbtInt intversion)
        {
            if (intversion.Value >= 2711)
                return new Java1p17Version();
            if (intversion.Value >= 2709)
                return new Java1p17SnapshotVersion();
            if (intversion.Value >= 2562) // 1.16 pre-6
                return new Java1p16Version();
            if (intversion.Value >= 1128) // 17w17a
                return new Java1p12Version();
            if (intversion.Value >= 503)
                return new Java1p10Version();
        }
        if (leveldat["GameRules"]?["doEntityDrops"] != null) // 1.8.1 pre-1
            return new Java1p8Version();
        if (leveldat["Player"]?["HealF"] != null) // 1.6.4, not great (ideally 13w42a, with another check for 13w43a)
            return new Java1p7Version();
        if (leveldat["MapFeatures"] != null)
            return new JavaB1p8Version();
        throw new InvalidOperationException("Couldn't determine world version, or it's from an old version from before maps existed (pre-beta 1.8)");
    }

    public override void AddStructure(StructureGrid structure, Inventory? inventory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Inventory> GetInventories()
    {
        yield return new LocalInventory();
        foreach (var file in Directory.GetFiles(Path.Combine(Folder, "playerdata"), "*.dat"))
        {
            if (Path.GetFileNameWithoutExtension(file).Length == 36)
                yield return new UUIDInventory(file);
        }
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
        var image = Version.Colors.Decode(colors);
        ProcessImage(image, new ProcessSettings(null, new EuclideanAlgorithm()));
        return new Map(id, new MapData(image, colors));
    }

    public override void AddMaps(IEnumerable<Map> maps)
    {
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = true };
            var data = Version.NbtFormat.CreateMapCompound(map.Data);
            data.Name = "data";
            nbt.GetRootTag<NbtCompound>().Add(data);
            nbt.SaveToFile(Path.Combine(Folder, "data", $"map_{map.ID}.dat"), NbtCompression.GZip);
        }
    }

    protected override void ProcessImage(Image<Rgba32> image, ProcessSettings settings)
    {
        var palette = Version.Colors.GetPalette();
        var quantizer = new CustomQuantizer(new QuantizerOptions() { Dither = settings.Dither }, palette, settings.Algorithm);
        image.Mutate(x => x.Quantize(quantizer));
    }

    protected override byte[] EncodeColors(Image<Rgba32> image) => Version.Colors.EncodeColors(image);
}
