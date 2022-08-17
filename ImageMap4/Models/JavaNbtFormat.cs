using System;
using System.IO;
using fNbt;

namespace ImageMap4;

public interface IJavaNbtFormat
{
    NbtCompound CreateMapCompound(MapData map);
    NbtCompound MakeMapItem(long id);
    bool StructuresSupported { get; }
    NbtCompound CreateStructureFile(long?[,] mapids);
    string StructureFileLocation(string world_folder, string identifier);
}

public abstract class AbstractNbtFormat : IJavaNbtFormat
{
    public abstract NbtCompound CreateMapCompound(MapData map);
    public abstract NbtCompound MakeMapItem(long id);
    public virtual bool StructuresSupported => true;
    protected abstract NbtCompound MakeItemFrame();
    public virtual NbtCompound CreateStructureFile(long?[,] mapids)
    {
        var entities = new NbtList();
        for (int y = 0; y < mapids.GetLength(0); y++)
        {
            for (int x = 0; x < mapids.GetLength(1); x++)
            {
                long? val = mapids[y, x];
                if (val.HasValue)
                {
                    var frame = MakeItemFrame();
                    frame.Name = "nbt";
                    var item = MakeMapItem(val.Value);
                    item.Name = "Item";
                    frame.Add(item);
                    entities.Add(new NbtCompound()
                    {
                        frame,
                        new NbtList("blockPos") { new NbtInt(0), new NbtInt(y), new NbtInt(x) }
                    });
                }
            }
        }
        return new NbtCompound() {
            new NbtList("size") {
                new NbtInt(1), new NbtInt(mapids.GetLength(0)), new NbtInt(mapids.GetLength(1))
            },
            entities,
            new NbtList("blocks") {
                new NbtCompound() {
                    new NbtList("pos") { new NbtInt(0), new NbtInt(0), new NbtInt(0) },
                    new NbtInt("state", 0)
                }
            },
            new NbtList("palette") {
                new NbtCompound() {
                    new NbtString("Name", "minecraft:air")
                }
            }
        };
    }
    public abstract string StructureFileLocation(string world_folder, string identifier);
}

public static class JavaMapMethods
{
    public static NbtCompound Beta1p8(MapData map)
    {
        return new NbtCompound
        {
            new NbtByteArray("colors", map.Colors),
            new NbtByte("scale", 0),
            new NbtByte("dimension", 0),
            new NbtByte("locked", 1),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Image.Width),
            new NbtShort("width", (short)map.Image.Height),
            new NbtByte("trackingPosition", 0),
            new NbtByte("unlimitedTracking", 0)
        };
    }
    public static NbtCompound Release1p16(MapData map)
    {
        return new NbtCompound
        {
            new NbtByteArray("colors", map.Colors),
            new NbtByte("scale", 0),
            new NbtString("dimension", "minecraft:overworld"),
            new NbtByte("locked", 1),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtByte("trackingPosition", 0),
            new NbtByte("unlimitedTracking", 0)
        };
    }
}

public abstract class NoStructuresNbtFormat : AbstractNbtFormat
{
    public override bool StructuresSupported => false;
    public override NbtCompound CreateStructureFile(long?[,] mapids)
    {
        throw new NotSupportedException();
    }
    public override string StructureFileLocation(string world_folder, string identifier)
    {
        throw new NotSupportedException();
    }
    protected override NbtCompound MakeItemFrame()
    {
        throw new NotSupportedException();
    }
}

public class Beta1p8NbtFormat : NoStructuresNbtFormat
{
    public override NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Beta1p8(map);
    public override NbtCompound MakeMapItem(long id)
    {
        return new NbtCompound
        {
            new NbtShort("id", 358),
            new NbtShort("Damage", (short)id),
            new NbtByte("Count", 1)
        };
    }
}

public class Release1p8NbtFormat : NoStructuresNbtFormat
{
    public override NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Beta1p8(map);
    public override NbtCompound MakeMapItem(long id)
    {
        return new NbtCompound
        {
            new NbtString("id", "minecraft:filled_map"),
            new NbtShort("Damage", (short)id),
            new NbtByte("Count", 1)
        };
    }
}

public class Release1p10NbtFormat : AbstractNbtFormat
{
    public override NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Beta1p8(map);
    public override bool StructuresSupported => true;
    public override NbtCompound MakeMapItem(long id)
    {
        return new NbtCompound
        {
            new NbtString("id", "minecraft:filled_map"),
            new NbtShort("Damage", (short)id),
            new NbtByte("Count", 1)
        };
    }
    protected override NbtCompound MakeItemFrame()
    {
        return new NbtCompound() {
            new NbtString("id", "ItemFrame"),
            new NbtByte("Facing", 1),
            new NbtByte("Fixed", 1),
            new NbtByte("Invulnerable", 1)
        };
    }
    public override string StructureFileLocation(string world_folder, string identifier)
    {
        return Path.Combine(world_folder, "structures", identifier.Replace(':', '_') + ".nbt");
    }
}

public class Snapshot16w32aNbtFormat : Release1p10NbtFormat
{
    protected override NbtCompound MakeItemFrame()
    {
        return new NbtCompound() {
            new NbtString("id", "minecraft:item_frame"),
            new NbtByte("Facing", 1),
            new NbtByte("Fixed", 1),
            new NbtByte("Invulnerable", 1)
        };
    }
}

public class Release1p16NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Release1p16(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
    public string StructureFileLocation(string world_folder, string identifier)
    {
        int colon = identifier.IndexOf(':');
        return Path.Combine(world_folder, "generated", identifier[..colon], identifier[(colon + 1)..] + ".nbt");
    }
}
