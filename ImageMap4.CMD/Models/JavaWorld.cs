using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using TryashtarUtils.Nbt;

namespace ImageMap4;

public class JavaWorld : World
{
    public IJavaVersion? Version { get; }
    public override string Name { get; }
    public override string WorldIcon { get; }
    public override DateTime AccessDate { get; }
    private readonly NbtFile LevelDat;

    public override bool SupportsInvisibleFrames => Version?.SupportsInvisibleFrames ?? false;
    public override bool SupportsGlowFrames => Version?.SupportsGlowFrames ?? false;
    public override bool SupportsChests => Version?.SupportsChests ?? false;
    public override bool SupportsStructures => Version?.SupportsStructures ?? false;

    public JavaWorld(string folder) : base(folder)
    {
        LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
        var version = VersionManager.DetermineJavaVersion(LevelDat.GetRootTag<NbtCompound>().Get<NbtCompound>("Data"));
        Version = version;
        Name = LevelDat.RootTag["Data"]?["LevelName"]?.StringValue ?? "";
        WorldIcon = Path.Combine(Folder, "icon.png");
        AccessDate = File.GetLastWriteTime(LevelDat.FileName);
    }

    public override bool IsIdTaken(long id)
    {
        string path = Version.MapFileLocation(this.Folder, id);
        return File.Exists(path);
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

    public override void AddChest(IEnumerable<long> ids, IInventory inventory)
    {
        if (Version == null)
        {
            return;
        }
        var items = new List<NbtCompound>();
        var idlist = ids.ToList();
        if (idlist.Count == 1 || !SupportsChests)
        {
            items.AddRange(idlist.Select(x => Version.MakeMapItem(x)));
        }
        else
        {
            var chunks = idlist.Chunk(27);
            foreach (var chunk in chunks)
            {
                var maps = idlist.Select(x => Version.MakeMapItem(x)).ToArray();
                for (int i = 0; i < maps.Length; i++)
                {
                    if (Version.DataVersion != null && Version.DataVersion.Value >= 3821)
                    {
                        maps[i].Name = "item";
                        maps[i] = new NbtCompound()
                        {
                            maps[i],
                            new NbtInt("slot", i)
                        };
                    }
                    else
                    {
                        maps[i].Add(new NbtByte("Slot", (byte)i));
                    }
                }
                var chest = Version.MakeChestItem($"maps {chunk[0]} - {chunk[chunk.Length - 1]}", maps);
                items.Add(chest);
            }
        }
        inventory.AddItems(items);
    }

    private static string ToStringUUID(int[] bits)
    {
        byte[] bytes = bits.SelectMany(BitConverter.GetBytes).ToArray();
        var guid = new Guid(bytes);
        return guid.ToString();
    }

    public override IEnumerable<IInventory> GetInventories()
    {
        var playerdata_folder = Path.GetDirectoryName(Version.PlayerDataLocation(Folder, "A"));
        var singleplayer_uuid = LevelDat.GetRootTag<NbtCompound>()?.Get<NbtCompound>("Data")?.Get<NbtIntArray>("singleplayer_uuid");
        if (singleplayer_uuid != null)
        {
            var uuid = ToStringUUID(singleplayer_uuid.Value);
            var matching_path = Version.PlayerDataLocation(Folder, uuid);
            if (Path.Exists(matching_path))
            {
                var player_file = new NbtFile(matching_path);
                var player_inv = LevelDat.GetRootTag<NbtCompound>()?.Get<NbtList>("Inventory");
                if (player_inv != null)
                {
                    yield return new JavaInventory("Local player", player_file, player_inv);
                }
            }
        }
        else
        {
            var singleplayer_inv = LevelDat.GetRootTag<NbtCompound>()?.Get<NbtCompound>("Data")?.Get<NbtCompound>("Player")?.Get<NbtList>("Inventory");
            if (singleplayer_inv != null)
            {
                yield return new JavaInventory("Local player", LevelDat, singleplayer_inv);
            }
        }
        if (Directory.Exists(playerdata_folder))
        {
            foreach (var file in Directory.GetFiles(playerdata_folder, "*.dat"))
            {
                string uuid = Path.GetFileNameWithoutExtension(file);
                if (uuid.Length == 36)
                {
                    var player_file = new NbtFile(file);
                    var player_inv = LevelDat.GetRootTag<NbtCompound>()?.Get<NbtList>("Inventory");
                    if (player_inv != null)
                    {
                        yield return new JavaInventory(uuid, player_file, player_inv);
                    }
                }
            }
        }
    }

    public override async IAsyncEnumerable<Map> GetMapsAsync()
    {
        var maps_folder = Path.GetDirectoryName(Version.MapFileLocation(Folder, 0));
        if (Directory.Exists(maps_folder))
        {
            foreach (var file in Directory.EnumerateFiles(maps_folder, "*.dat"))
            {
                if (GetMapFileId(file, out _)) {
                    yield return await Task.Run(() => GetMap(file));
                }
            }
        }
    }

    private static bool GetMapFileId(string filepath, out long? id)
    {
        string name = Path.GetFileNameWithoutExtension(filepath);
        if (name.StartsWith("map_"))
        {
            name = name[4..];
        }
        if (long.TryParse(name, out long gotid))
        {
            id = gotid;
            return true;
        }
        id = null;
        return false;
    }

    private Map GetMap(string file)
    {
        GetMapFileId(file, out long? id);
        var nbt = new NbtFile() { BigEndian = true };
        nbt.LoadFromFile(file, NbtCompression.GZip, null);
        var full_data = nbt.GetRootTag<NbtCompound>();
        var colors = full_data.Get<NbtCompound>("data").Get<NbtByteArray>("colors").Value;
        var image = Version.Decode(colors);
        return new Map(id.Value, new MapData(image, colors, full_data));
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
            string file = Version.MapFileLocation(this.Folder, map.ID);
            string folder = Path.GetDirectoryName(file);
            Directory.CreateDirectory(folder);
            nbt.SaveToFile(file, NbtCompression.GZip);
        }
        var maps_folder = Path.GetDirectoryName(Version.MapFileLocation(Folder, 0));
        if (Directory.Exists(maps_folder))
        {
            long biggest_id = 0;
            foreach (var file in Directory.EnumerateFiles(maps_folder, "*.dat"))
            {
                if (GetMapFileId(file, out long? id))
                {
                    if (id != null)
                    {
                        biggest_id = Math.Max(biggest_id, id.Value);
                    }
                }
            }
            var idcount = Version.MapLastIdLocation(Folder);
            Directory.CreateDirectory(Path.GetDirectoryName(idcount));
            var compound = new NbtCompound("")
            {
                new NbtCompound("data")
                {
                    new NbtInt("map", (int)biggest_id)
                },
                new NbtInt("DataVersion", Version.DataVersion.Value)
            };
            var nbtfile = new NbtFile(compound);
            nbtfile.SaveToFile(idcount, NbtCompression.GZip);
        }
    }

    public override void RemoveMaps(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            var file = Version.MapFileLocation(this.Folder, id);
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
