using fNbt;
using LevelDBWrapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public abstract void AddMaps(IEnumerable<Map> maps);
    protected abstract void ProcessImage(Image<Rgba32> image, ProcessSettings settings);
    protected abstract byte[] EncodeColors(Image<Rgba32> image);
    public IEnumerable<MapData> MakeMaps(ImportSettings settings)
    {
        using var image = Image.Load<Rgba32>(settings.Preview.Source);
        image.Mutate(x =>
        {
            x.Rotate((float)settings.Preview.Rotation);
            if (settings.Preview.ScaleX == -1)
                x.Flip(FlipMode.Horizontal);
            if (settings.Preview.ScaleY == -1)
                x.Flip(FlipMode.Vertical);
            x.Resize(new ResizeOptions()
            {
                Size = new(128 * settings.Width, 128 * settings.Height),
                Sampler = settings.Sampler,
                Mode = settings.ResizeMode
            });
            // uniform scale crops to content, so we need to re-add transparency to get correct size
            x.Resize(new ResizeOptions()
            {
                Size = new(128 * settings.Width, 128 * settings.Height),
                Mode = ResizeMode.BoxPad
            });
        });
        var original = Split(image, settings.Width, settings.Height);
        ProcessImage(image, settings.ProcessSettings);
        var finished = Split(image, settings.Width, settings.Height);
        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                yield return new MapData(finished[x, y], original[x, y], EncodeColors(finished[x, y]));
            }
        }
    }

    private static Image<Rgba32>[,] Split(Image<Rgba32> source, int columns, int rows)
    {
        var result = new Image<Rgba32>[columns, rows];
        int width = source.Width / columns;
        int height = source.Height / rows;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var tile = new Image<Rgba32>(width, height);
                source.ProcessPixelRows(tile, (sa, ta) =>
                {
                    for (int i = 0; i < height; i++)
                    {
                        var source = sa.GetRowSpan(height * y + i);
                        var target = ta.GetRowSpan(i);
                        source.Slice(width * x, width).CopyTo(target);
                    }
                });
                result[x, y] = tile;
            }
        }
        return result;
    }
}

public class JavaWorld : World
{
    public readonly NbtFile LevelDat;
    public IJavaVersion Version { get; }
    public JavaWorld(string folder) : base(folder)
    {
        LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
        Version = DetermineVersionFromLevelDat(LevelDat.RootTag.Get<NbtCompound>("Data"));
        Name = LevelDat.RootTag["Data"]?["LevelName"]?.StringValue;
        WorldIcon = Path.Combine(Folder, "icon.png");
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
        }
        if (leveldat["GameRules"]?["doEntityDrops"] != null) // 1.8.1 pre-1
            return new Java1p8Version();
        if (leveldat["Player"]?["HealF"] != null) // 1.6.4, not great (ideally 13w42a, with another check for 13w43a)
            return new Java1p7Version();
        if (leveldat["MapFeatures"] != null)
            return new JavaB1p8Version();
        throw new InvalidOperationException("Couldn't determine world version, or it's from an old version from before maps existed (pre-beta 1.8)");
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
                    var nbt = new NbtFile() { BigEndian = true };
                    nbt.LoadFromFile(file, NbtCompression.GZip, null);
                    var colors = nbt.RootTag.Get<NbtCompound>("data").Get<NbtByteArray>("colors").Value;
                    var image = Version.Decode(colors);
                    ProcessImage(image, new ProcessSettings(null, new EuclideanAlgorithm()));
                    yield return new Map(id, new MapData(image, colors));
                }
            }
        }
    }

    public override void AddMaps(IEnumerable<Map> maps)
    {
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = true };
            var data = Version.CreateMapCompound(map.Data);
            data.Name = "data";
            nbt.RootTag.Add(data);
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

    public override IEnumerable<Map> GetMaps()
    {
        using var db = OpenDB();
        using var iterator = db.CreateIterator();
        iterator.Seek("map_");
        while (iterator.IsValid())
        {
            var name = iterator.StringKey();
            if (name.StartsWith("map_"))
            {
                long id = long.Parse(name[4..]);
                var bytes = iterator.Value();
                var nbt = new NbtFile() { BigEndian = false };
                nbt.LoadFromBuffer(bytes, 0, bytes.Length, NbtCompression.None);
                var colors = nbt.RootTag.Get<NbtByteArray>("colors").Value;
                var image = Image.LoadPixelData<Rgba32>(colors, 128, 128);
                yield return new Map(id, new MapData(image, colors));
            }
            else
                break;
            iterator.Next();
        }
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
