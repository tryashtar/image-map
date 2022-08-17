using System;
using fNbt;

namespace ImageMap4;

public interface IJavaNbtFormat
{
    NbtCompound CreateMapCompound(MapData map);
    NbtCompound CreateStructureFile(long?[,] ids);
    bool StructuresSupported { get; }
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
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Image.Width),
            new NbtShort("width", (short)map.Image.Height)
        };
    }
    public static NbtCompound Release1p12(MapData map)
    {
        return new NbtCompound
        {
            new NbtByteArray("colors", map.Colors),
            new NbtByte("scale", 0),
            new NbtByte("dimension", 0),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Image.Width),
            new NbtShort("width", (short)map.Image.Height),
            new NbtByte("trackingPosition", 0),
            new NbtByte("unlimitedTracking", 0)
        };
    }
    public static NbtCompound Release1p13(MapData map)
    {
        return new NbtCompound
        {
            new NbtByteArray("colors", map.Colors),
            new NbtByte("scale", 0),
            new NbtByte("dimension", 0),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtByte("trackingPosition", 0),
            new NbtByte("unlimitedTracking", 0)
        };
    }
    public static NbtCompound Release1p14(MapData map)
    {
        return new NbtCompound
        {
            new NbtByteArray("colors", map.Colors),
            new NbtByte("scale", 0),
            new NbtByte("dimension", 0),
            new NbtByte("locked", 1),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
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

public static class JavaStructureMethods
{
    public static NbtCompound Release1p10(long?[,] mapids)
    {
        var entities = new NbtList();
        for (int y = 0; y < mapids.GetLength(0); y++)
        {
            for (int x = 0; x < mapids.GetLength(1); x++)
            {
                long? val = mapids[y, x];
                if (val.HasValue)
                {
                    entities.Add(new NbtCompound()
                    {
                        new NbtCompound("nbt") {
                            new NbtString("id", "ItemFrame"),
                            new NbtByte("Facing", 1),
                            new NbtByte("Invulnerable", 1),
                            new NbtCompound("Item") {
                                new NbtString("id", "minecraft:filled_map"),
                                new NbtByte("Count", 1),
                                new NbtShort("Damage", (short)val.Value)
                            }
                        }
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
}

public class Beta1p8NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Beta1p8(map);
    public bool StructuresSupported => false;
    public NbtCompound CreateStructureFile(long?[,] mapids)
    {
        throw new NotSupportedException();
    }
}

public class Release1p10NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Beta1p8(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
}

public class Release1p12NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Release1p12(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
}

public class Release1p13NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Release1p13(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
}

public class Release1p14NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Release1p14(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
}

public class Release1p16NbtFormat : IJavaNbtFormat
{
    public NbtCompound CreateMapCompound(MapData map) => JavaMapMethods.Release1p16(map);
    public bool StructuresSupported => true;
    public NbtCompound CreateStructureFile(long?[,] mapids) => JavaStructureMethods.Release1p10(mapids);
}
