﻿using fNbt;
using LevelDBWrapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
    protected abstract void PrepareImage(Image<Rgba32> image);
    public IEnumerable<Map> MakeMaps(ImportSettings settings)
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
        PrepareImage(image);
        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                var tile = new Image<Rgba32>(128, 128);
                for (int i = 0; i < 128; i++)
                {
                    image.ProcessPixelRows(tile, (sa, ta) =>
                    {
                        var source = sa.GetRowSpan(128 * y + i);
                        var target = ta.GetRowSpan(i);
                        source.Slice(128 * x, 128).CopyTo(target);
                    });
                }
                yield return new Map(0, new ImageSharpImageSource<Rgba32>(tile));
            }
        }
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
                    yield return new Map(id, new ImageSharpImageSource<Rgba32>(image));
                }
            }
        }
    }

    protected override void PrepareImage(Image<Rgba32> image)
    {
        
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
                var colors = nbt.RootTag.Get<NbtByteArray>("colors").Value;
                bool blank = true;
                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] != 0)
                    {
                        blank = false;
                        break;
                    }
                }
                if (!blank)
                {
                    var image = Image.LoadPixelData<Rgba32>(colors, 128, 128);
                    yield return new Map(id, new ImageSharpImageSource<Rgba32>(image));
                }
            }
            else
                break;
            iterator.Next();
        }
    }

    protected override void PrepareImage(Image<Rgba32> image)
    {

    }

    private LevelDB OpenDB()
    {
        return new LevelDB(Path.Combine(Folder, "db"));
    }
}
