using fNbt;
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
    public readonly NbtFile File;
    public readonly NbtList Inventory;
    public JavaInventory(string name, NbtFile file, NbtList inventory)
    {
        Name = name;
        File = file;
        Inventory = inventory;
    }

    public void AddItems(IEnumerable<NbtCompound> items)
    {
        var occupied_slots = Inventory.Cast<NbtCompound>().Select(x => x.Get<NbtByte>("Slot").Value).ToHashSet();
        foreach (var item in items)
        {
            for (byte i = 0; i < 36; i++)
            {
                if (!occupied_slots.Contains(i))
                {
                    item.Add(new NbtByte("Slot", i));
                    Inventory.Add(item);
                    occupied_slots.Add(i);
                    break;
                }
            }
        }
        File.SaveToFile(File.FileName, File.FileCompression);
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
