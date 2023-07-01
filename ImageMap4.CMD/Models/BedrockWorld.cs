using fNbt;
using LevelDBWrapper;

namespace ImageMap4;

public class BedrockWorld : IWorld
{
    private LevelDB? DBAccess;
    private readonly object DBLock = new();
    public IBedrockVersion Version { get; }
    public string Name { get; }
    public string VersionName { get; }
    public string Folder { get; }
    public Image<Rgba32>? WorldIcon { get; }
    public DateTime AccessDate { get; }

    public BedrockWorld(string folder)
    {
        Folder = folder;
        string file = Path.Combine(folder, "levelname.txt");
        if (File.Exists(file))
            Name = File.ReadLines(file).FirstOrDefault() ?? "";
        else
            Name = "";
        string icon = Path.Combine(Folder, "world_icon.jpeg");
        if (File.Exists(icon))
            WorldIcon = Image.Load<Rgba32>(icon);
        using var leveldat = File.OpenRead(Path.Combine(folder, "level.dat"));
        leveldat.Position = 8;
        var nbt = new NbtFile() { BigEndian = false };
        nbt.LoadFromStream(leveldat, NbtCompression.None);
        var root = nbt.GetRootTag<NbtCompound>();
        Version = VersionManager.DetermineBedrockVersion(root) ??
                  throw new InvalidDataException("Could not determine version of world");
        var versiontag = root.Get<NbtList>("lastOpenedWithVersion");
        VersionName = versiontag != null
            ? String.Join('.', versiontag.Select(x => x.IntValue.ToString()))
            : Version.ToString();
        AccessDate = File.GetLastWriteTime(leveldat.Name);
    }

    public bool IsIdTaken(long id)
    {
        var db = OpenDB();
        return db.Get($"map_{id}") != null;
    }

    public void AddStructures(IEnumerable<StructureGrid> structures, IInventory inventory)
    {
        var db = OpenDB();
        using var batch = new WriteBatch();
        var items = new List<NbtCompound>();
        foreach (var structure in structures)
        {
            var blockdata = new NbtCompound("block_position_data");
            var mapids = structure.ToIDGrid();
            for (int y = 0; y < structure.GridHeight; y++)
            {
                for (int x = 0; x < structure.GridWidth; x++)
                {
                    long? id = mapids[x, structure.GridHeight - y - 1];
                    if (id.HasValue)
                    {
                        var mapitem = Version.CreateMapItem(id.Value);
                        mapitem.Name = "Item";
                        blockdata.Add(new NbtCompound((y * structure.GridWidth + x).ToString())
                        {
                            new NbtCompound("block_entity_data")
                            {
                                new NbtString("id", structure.GlowingFrames ? "GlowItemFrame" : "ItemFrame"),
                                mapitem
                            }
                        });
                    }
                }
            }

            var nbt = new NbtCompound("")
            {
                new NbtInt("format_version", 1),
                new NbtList("size")
                    { new NbtInt(1), new NbtInt(structure.GridHeight), new NbtInt(structure.GridWidth) },
                new NbtCompound("structure")
                {
                    new NbtList("block_indices")
                    {
                        new NbtList(Enumerable.Repeat(0, structure.GridHeight * structure.GridWidth)
                            .Select(x => new NbtInt(x))),
                        new NbtList(Enumerable.Repeat(-1, structure.GridHeight * structure.GridWidth)
                            .Select(x => new NbtInt(x)))
                    },
                    new NbtList("entities"),
                    new NbtCompound("palette")
                    {
                        new NbtCompound("default")
                        {
                            new NbtList("block_palette")
                            {
                                new NbtCompound()
                                {
                                    new NbtString("name",
                                        structure.GlowingFrames ? "minecraft:glow_frame" : "minecraft:frame"),
                                    new NbtCompound("states")
                                    {
                                        new NbtInt("facing_direction", 4),
                                        new NbtByte("item_frame_map_bit", 1)
                                    }
                                }
                            },
                            blockdata
                        }
                    }
                },
                new NbtList("structure_world_origin") { new NbtInt(0), new NbtInt(0), new NbtInt(0) }
            };
            var key = "structuretemplate_" + structure.Identifier;
            var file = new NbtFile(nbt) { BigEndian = false };
            batch.Put(key, file.SaveToBuffer(NbtCompression.None));
            var item = new NbtCompound
            {
                new NbtString("Name", "minecraft:structure_block"),
                new NbtByte("Count", 1),
                new NbtCompound("tag")
                {
                    new NbtInt("data", 2),
                    new NbtString("structureName", structure.Identifier),
                    new NbtFloat("integrity", 100),
                    new NbtInt("xStructureOffset", 0),
                    new NbtInt("yStructureOffset", 0),
                    new NbtInt("zStructureOffset", 0),
                    new NbtInt("xStructureSize", 1),
                    new NbtInt("yStructureSize", structure.GridHeight),
                    new NbtInt("zStructureSize", structure.GridWidth),
                    new NbtCompound("display")
                    {
                        new NbtString("Name", $"§r§d{structure.Identifier}§r")
                    }
                }
            };
            items.Add(item);
        }

        db.Write(batch);
        inventory.AddItems(items);
    }

    public IEnumerable<IInventory> GetInventories()
    {
        yield return new BedrockInventory("Local player", this, "~local_player");
        var db = OpenDB();
        using var iterator = db.CreateIterator();
        iterator.Seek("player_server_");
        while (iterator.IsValid())
        {
            var name = iterator.StringKey();
            if (name.StartsWith("player_server_"))
                yield return new BedrockInventory(name[15..], this, name);
            else
                break;
            iterator.Next();
        }
    }

    public async IAsyncEnumerable<Map> GetMapsAsync()
    {
        var db = OpenDB();
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
        var full_data = nbt.GetRootTag<NbtCompound>();
        var colors = full_data.Get<NbtByteArray>("colors").Value;
        var image = Image.LoadPixelData<Rgba32>(colors, 128, 128);
        return new Map(id, new MapData(image, colors, full_data));
    }

    public void AddMaps(IEnumerable<Map> maps)
    {
        var db = OpenDB();
        using var batch = new WriteBatch();
        foreach (var map in maps)
        {
            var nbt = new NbtFile { BigEndian = false };
            var data = map.Data.FullData ?? Version.CreateMapCompound(map);
            data["mapId"] = new NbtLong(map.ID);
            data.Name = "";
            nbt.RootTag = data;
            var bytes = nbt.SaveToBuffer(NbtCompression.None);
            batch.Put($"map_{map.ID}", bytes);
        }

        db.Write(batch);
    }

    public void RemoveMaps(IEnumerable<long> ids)
    {
        var db = OpenDB();
        using var batch = new WriteBatch();
        foreach (var id in ids)
        {
            batch.Delete($"map_{id}");
        }

        db.Write(batch);
    }

    public void ProcessImage(Image<Rgba32> image, ProcessSettings settings)
    {
        // no quantization needed, Bedrock supports all colors
    }

    public byte[] EncodeColors(Image<Rgba32> image)
    {
        var result = new byte[128 * 128 * 4];
        image.CopyPixelDataTo(result);
        return result;
    }

    // we definitely only want one LevelDB instance accessing this world at a time
    // it's nice to open on demand and dispose it when you're done
    // but that's not very threadsafe (e.g. GetMapsAsync)
    // seems to work ok for now, but could be worth taking another look at
    public LevelDB OpenDB()
    {
        lock (DBLock)
        {
            if (DBAccess == null || DBAccess.Disposed)
                DBAccess = new LevelDB(Path.Combine(Folder, "db"),
                    new Options { CompressionLevel = LevelDBWrapper.CompressionLevel.ZlibRawCompression });
            return DBAccess;
        }
    }

    public void CloseDB()
    {
        lock (DBLock)
        {
            DBAccess?.Dispose();
        }
    }
}