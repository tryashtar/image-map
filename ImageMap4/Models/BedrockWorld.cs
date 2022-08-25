using fNbt;
using LevelDBWrapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMap4;

public class BedrockWorld : World
{
    private LevelDB DBAccess;
    private object DBLock = new();
    public IBedrockVersion Version { get; }
    public override string Name { get; }
    public override string WorldIcon { get; }
    public override DateTime AccessDate { get; }

    public BedrockWorld(string folder) : base(folder)
    {
        string file = Path.Combine(folder, "levelname.txt");
        if (File.Exists(file))
            Name = File.ReadLines(file).FirstOrDefault() ?? "";
        else
            Name = "";
        WorldIcon = Path.Combine(Folder, "world_icon.jpeg");
        using var leveldat = File.OpenRead(Path.Combine(folder, "level.dat"));
        leveldat.Position = 8;
        var nbt = new NbtFile() { BigEndian = false };
        nbt.LoadFromStream(leveldat, NbtCompression.None);
        Version = VersionManager.DetermineBedrockVersion(nbt.GetRootTag<NbtCompound>());
        AccessDate = File.GetLastWriteTime(leveldat.Name);
    }

    public override void AddStructure(StructureGrid structure, IInventory inventory)
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
                    blockdata.Add(new NbtCompound((y * structure.GridWidth + x).ToString()) {
                        new NbtCompound("block_entity_data") {
                            new NbtString("id", structure.GlowingFrames ? "GlowItemFrame" : "ItemFrame"),
                            new NbtCompound("Item") {
                                new NbtString("Name", "minecraft:filled_map"),
                                new NbtByte("Count", 1),
                                new NbtCompound("tag") {
                                    new NbtLong("map_uuid", id.Value)
                                }
                            }
                        }
                    });
                }
            }
        }
        var nbt = new NbtCompound("") {
            new NbtInt("format_version", 1),
            new NbtList("size") { new NbtInt(1), new NbtInt(structure.GridHeight), new NbtInt(structure.GridWidth) },
            new NbtCompound("structure") {
                new NbtList("block_indices") {
                    new NbtList(Enumerable.Repeat(0, structure.GridHeight * structure.GridWidth).Select(x => new NbtInt(x))),
                    new NbtList(Enumerable.Repeat(-1, structure.GridHeight * structure.GridWidth).Select(x => new NbtInt(x)))
                },
                new NbtList("entities"),
                new NbtCompound("palette") {
                    new NbtCompound("default") {
                        new NbtList("block_palette") {
                            new NbtCompound() {
                                new NbtString("name", structure.GlowingFrames ? "minecraft:glow_frame" : "minecraft:frame"),
                                new NbtCompound("states") {
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
        var db = OpenDB();
        db.Put(key, file.SaveToBuffer(NbtCompression.None));
        var item = new NbtCompound {
            new NbtString("Name", "minecraft:structure_block"),
            new NbtByte("Count", 1),
            new NbtCompound("tag") {
                new NbtInt("data", 2),
                new NbtString("structureName", structure.Identifier),
                new NbtInt("xStructureOffset", 0),
                new NbtInt("yStructureOffset", 0),
                new NbtInt("zStructureOffset", 0),
                new NbtInt("xStructureSize", 1),
                new NbtInt("yStructureSize", structure.GridHeight),
                new NbtInt("zStructureSize", structure.GridWidth),
                new NbtCompound("display") {
                    new NbtString("Name", $"§r§d{structure.Identifier}§r")
                }
            }
        };
        inventory.AddItem(item);
    }

    public override IEnumerable<IInventory> GetInventories()
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

    public override async IAsyncEnumerable<Map> GetMapsAsync()
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

    public override void AddMaps(IEnumerable<Map> maps)
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

    public override void RemoveMaps(IEnumerable<long> ids)
    {
        var db = OpenDB();
        foreach (var id in ids)
        {
            db.Delete($"map_{id}");
        }
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

    // we definitely only want one LevelDB instance accessing this world at a time
    // it's nice to open on demand and dispose it when you're done
    // but that's not very threadsafe (e.g. GetMapsAsync)
    // seems to work ok for now, but could be worth taking another look at
    public LevelDB OpenDB()
    {
        lock (DBLock)
        {
            Debug.WriteLine($"Opening {Name}");
            if (DBAccess == null || DBAccess.Disposed)
                DBAccess = new LevelDB(Path.Combine(Folder, "db"));
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
