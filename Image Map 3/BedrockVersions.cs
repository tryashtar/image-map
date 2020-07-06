using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public interface IBedrockVersion
    {
        NbtCompound CreateMapItem(byte slot, long mapid);
        NbtCompound CreateMapCompound(long mapid, byte[] colors);
    }

    public abstract class AbstractBedrockVersion : IBedrockVersion
    {
        public abstract NbtCompound CreateMapCompound(long mapid, byte[] colors);
        public abstract NbtCompound CreateMapItem(byte slot, long mapid);
    }

    public class Bedrock1p2Version:AbstractBedrockVersion
    {
        public static Bedrock1p2Version Instance = new Bedrock1p2Version();
        private Bedrock1p2Version() { }

        public override NbtCompound CreateMapCompound(long mapid, byte[] colors)
        {
            return new NbtCompound
            {
                new NbtLong("mapId", mapid),
                new NbtLong("parentMapId", -1),
                new NbtByteArray("colors", colors),
                new NbtByte("scale", 4),
                new NbtByte("dimension", 0),
                new NbtByte("fullyExplored", 1),
                new NbtByte("unlimitedTracking", 0),
                new NbtInt("xCenter", Int32.MaxValue),
                new NbtInt("zCenter", Int32.MaxValue),
                new NbtShort("height", Map.MAP_HEIGHT),
                new NbtShort("width", Map.MAP_WIDTH)
            };
        }

        public override NbtCompound CreateMapItem(byte slot, long mapid)
        {
            return new NbtCompound
            {
                new NbtShort("id", 358),
                new NbtShort("Damage", 0),
                new NbtByte("Slot", slot),
                new NbtByte("Count", 1),
                new NbtCompound("tag")
                {
                    new NbtLong("map_uuid", mapid),
                    new NbtInt("map_name_index", (int)mapid)
                }
            };
        }

        public override string ToString() => "1.2+";
    }

    public class Bedrock1p7Version : AbstractBedrockVersion
    {
        public static Bedrock1p7Version Instance = new Bedrock1p7Version();
        private Bedrock1p7Version() { }

        public override NbtCompound CreateMapCompound(long mapid, byte[] colors) => Bedrock1p2Version.Instance.CreateMapCompound(mapid, colors);
        public override NbtCompound CreateMapItem(byte slot, long mapid)
        {
            return new NbtCompound
            {
                new NbtString("Name", "minecraft:map"),
                new NbtShort("Damage", 0),
                new NbtByte("Slot", slot),
                new NbtByte("Count", 1),
                new NbtCompound("tag")
                {
                    new NbtLong("map_uuid", mapid),
                    new NbtInt("map_name_index", (int)mapid)
                }
            };
        }

        public override string ToString() => "1.7+";
    }

    public class Bedrock1p11Version : AbstractBedrockVersion
    {
        public static Bedrock1p11Version Instance = new Bedrock1p11Version();
        private Bedrock1p11Version() { }

        public override NbtCompound CreateMapCompound(long mapid, byte[] colors)
        {
            return new NbtCompound
            {
                new NbtLong("mapId", mapid),
                new NbtLong("parentMapId", -1),
                new NbtByteArray("colors", colors),
                new NbtByte("mapLocked", 1),
                new NbtByte("scale", 4),
                new NbtByte("dimension", 0),
                new NbtByte("fullyExplored", 1),
                new NbtByte("unlimitedTracking", 0),
                new NbtInt("xCenter", Int32.MaxValue),
                new NbtInt("zCenter", Int32.MaxValue),
                new NbtShort("height", Map.MAP_HEIGHT),
                new NbtShort("width", Map.MAP_WIDTH)
            };
        }

        public override NbtCompound CreateMapItem(byte slot, long mapid) => Bedrock1p7Version.Instance.CreateMapItem(slot, mapid);

        public override string ToString() => "1.11+";
    }
}