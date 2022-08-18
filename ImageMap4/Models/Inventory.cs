using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;

namespace ImageMap4;

public interface IInventory
{
    string Name { get; }
    void AddItem(NbtCompound item);
}

public class NoInventory : IInventory
{
    public string Name => "None";
    public void AddItem(NbtCompound item) { }
}

public class JavaInventory : IInventory
{
    public string Name { get; }
    public readonly string FilePath;
    public readonly NbtPath DataPath;
    public JavaInventory(string name, string file, NbtPath path)
    {
        Name = name;
        FilePath = file;
        DataPath = path;
    }

    public void AddItem(NbtCompound item)
    {
        var file = new NbtFile(FilePath);
        var items = DataPath.Traverse(file.GetRootTag<NbtCompound>()).First() as NbtList;
        var occupied_slots = items.Cast<NbtCompound>().Select(x => x.Get<NbtByte>("Slot").Value).ToHashSet();
        for (byte i = 0; i < 36; i++)
        {
            if (!occupied_slots.Contains(i))
            {
                item.Add(new NbtByte("Slot", i));
                items.Add(item);
                break;
            }
        }
        file.SaveToFile(FilePath, file.FileCompression);
    }
}

public class BedrockInventory : IInventory
{
    public string Name { get; }
    public readonly BedrockWorld World;
    public readonly string Key;
    public BedrockInventory(string name, BedrockWorld world, string key)
    {
        Name = name;
        World = world;
        Key = key;
    }

    public void AddItem(NbtCompound item)
    {
        using var db = World.OpenDB();
        var bytes = db.Get(Key);
        var file = new NbtFile() { BigEndian = false };
        file.LoadFromBuffer(bytes, 0, bytes.Length, NbtCompression.None);
        var items = file.GetRootTag<NbtCompound>().Get<NbtList>("Inventory");
        foreach (NbtCompound slot in items.ToList())
        {
            if (slot.Get<NbtByte>("Count").Value == 0)
            {
                items.Remove(slot);
                item.Add((NbtByte)slot.Get<NbtByte>("Slot").Clone());
                items.Add(item);
                break;
            }
        }
        bytes = file.SaveToBuffer(NbtCompression.None);
        db.Put(Key, bytes);
    }
}
