using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMap4;

public interface IBedrockVersion
{
    NbtCompound CreateMapCompound(Map map);
}

public abstract class AbstractBedrockVersion : IBedrockVersion
{
    public abstract NbtCompound CreateMapCompound(Map map);
}

public class Bedrock1p2Version : AbstractBedrockVersion
{
    public override string ToString() => "1.2+";
    public override NbtCompound CreateMapCompound(Map map)
    {
        return new NbtCompound
        {
            new NbtLong("mapId", map.ID),
            new NbtLong("parentMapId", -1),
            new NbtByteArray("colors", map.Data.Colors),
            new NbtByte("scale", 4),
            new NbtByte("dimension", 0),
            new NbtByte("fullyExplored", 1),
            new NbtByte("unlimitedTracking", 0),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Data.Image.Height),
            new NbtShort("width", (short)map.Data.Image.Width)
        };
    }
}

public class Bedrock1p7Version : AbstractBedrockVersion
{
    public override string ToString() => "1.7+";
    public override NbtCompound CreateMapCompound(Map map)
    {
        return new NbtCompound
        {
            new NbtLong("mapId", map.ID),
            new NbtLong("parentMapId", -1),
            new NbtByteArray("colors", map.Data.Colors),
            new NbtByte("scale", 4),
            new NbtByte("dimension", 0),
            new NbtByte("fullyExplored", 1),
            new NbtByte("unlimitedTracking", 0),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Data.Image.Height),
            new NbtShort("width", (short)map.Data.Image.Width)
        };
    }
}

public class Bedrock1p11Version : AbstractBedrockVersion
{
    public override string ToString() => "1.11+";
    public override NbtCompound CreateMapCompound(Map map)
    {
        return new NbtCompound
        {
            new NbtLong("mapId", map.ID),
            new NbtLong("parentMapId", -1),
            new NbtByteArray("colors", map.Data.Colors),
            new NbtByte("mapLocked", 1),
            new NbtByte("scale", 4),
            new NbtByte("dimension", 0),
            new NbtByte("fullyExplored", 1),
            new NbtByte("unlimitedTracking", 0),
            new NbtInt("xCenter", Int32.MaxValue),
            new NbtInt("zCenter", Int32.MaxValue),
            new NbtShort("height", (short)map.Data.Image.Height),
            new NbtShort("width", (short)map.Data.Image.Width)
        };
    }
}
