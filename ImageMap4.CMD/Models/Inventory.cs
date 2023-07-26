using fNbt;
using LevelDBWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;

namespace ImageMap4;

public interface IInventory
{
    string Name { get; }
    void AddItems(IEnumerable<NbtCompound> items);
}

// I would prefer if we just used null for this, but we need something to show up for "Name" in the combobox
public class NoInventory : IInventory
{
    public string Name => "None";
    public void AddItems(IEnumerable<NbtCompound> items) { }
}

public class JavaInventory : IInventory
{
    public string Name { get; private set; }
    public readonly string FilePath;
    public readonly NbtPath DataPath;
    public JavaInventory(string name, string file, NbtPath path)
    {
        Name = name;
        FilePath = file;
        DataPath = path;
    }

    public void AddItems(IEnumerable<NbtCompound> items)
    {
        var file = new NbtFile(FilePath);
        var inventory = DataPath.Traverse(file.GetRootTag<NbtCompound>()).First() as NbtList;
        var occupied_slots = inventory.Cast<NbtCompound>().Select(x => x.Get<NbtByte>("Slot").Value).ToHashSet();
        foreach (var item in items)
        {
            for (byte i = 0; i < 36; i++)
            {
                if (!occupied_slots.Contains(i))
                {
                    item.Add(new NbtByte("Slot", i));
                    inventory.Add(item);
                    break;
                }
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
        // Name could also be a UUID, but it seems impossible to get a username from this
        Name = name;
        World = world;
        Key = key;
    }

    public void AddItems(IEnumerable<NbtCompound> items)
    {
        var db = World.OpenDB();
        var bytes = db.Get(Key);
        var file = new NbtFile() { BigEndian = false };
        file.LoadFromBuffer(bytes, 0, bytes.Length, NbtCompression.None);
        var inventory = file.GetRootTag<NbtCompound>().Get<NbtList>("Inventory");
        // remember bedrock saves empty slots
        foreach (var item in items)
        {
            foreach (NbtCompound slot in inventory.ToList())
            {
                if (slot.Get<NbtByte>("Count").Value == 0)
                {
                    inventory.Remove(slot);
                    item.Add((NbtByte)slot.Get<NbtByte>("Slot").Clone());
                    inventory.Add(item);
                    break;
                }
            }
        }
        bytes = file.SaveToBuffer(NbtCompression.None);
        db.Put(Key, bytes);
    }
}
