using fNbt;
using LevelDB;
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

public class JavaInventory : IInventory, INotifyPropertyChanged
{
    public string Name { get; private set; }
    public readonly string FilePath;
    public readonly NbtPath DataPath;
    public JavaInventory(string name, string file, NbtPath path)
    {
        Name = name;
        FilePath = file;
        DataPath = path;
        if (Name.Length == 36)
        {
            // convert UUIDs to playernames
            // first check in cache, which is like a slow dictionary where keys are in even places and values in odd
            bool found = false;
            if (Properties.Settings.Default.UsernameCache == null)
                Properties.Settings.Default.UsernameCache = new();
            for (int i = 0; i < Properties.Settings.Default.UsernameCache.Count; i += 2)
            {
                if (Properties.Settings.Default.UsernameCache[i] == Name)
                {
                    Name = Properties.Settings.Default.UsernameCache[i + 1];
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                // get name from Mojang's API
                // if it fails, just ignore
                // if it succeeds, cache it forever
                Debug.WriteLine($"Looking up UUID {Name}");
                var result = Client.GetAsync($"https://api.mojang.com/user/profiles/{Name}/names");
                result.ContinueWith(x =>
                {
                    if (x.IsCompletedSuccessfully)
                    {
                        var response = x.Result.Content.ReadAsStringAsync().Result;
                        var json = JsonDocument.Parse(response);
                        if (json.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            string newname = json.RootElement[json.RootElement.GetArrayLength() - 1].GetProperty("name").GetString();
                            lock (Properties.Settings.Default.UsernameCache)
                            {
                                Properties.Settings.Default.UsernameCache.Add(Name);
                                Properties.Settings.Default.UsernameCache.Add(newname);
                            }
                            this.Name = newname;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                        }
                    }
                });
            }
        }
    }
    private static readonly HttpClient Client = new();
    public event PropertyChangedEventHandler? PropertyChanged;

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
        var bytes = db.GetBytes(Key);
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
