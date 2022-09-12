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
    NbtCompound CreateMapItem(long id);
    NbtCompound CreateMapCompound(Map map);
}

public class BedrockVersionBuilder
{
    public string? Name;
    public NbtCompound? MapItem;
    public NbtCompound? MapData;
    public void Add(BedrockUpdate update)
    {
        this.Name = update.Name ?? this.Name;
        this.MapData = update.MapData ?? this.MapData;
        this.MapItem = update.MapItem ?? this.MapItem;
    }
    public IBedrockVersion Build()
    {
        NbtCompound data_maker(Map map)
        {
            var compound = (NbtCompound)MapData.Clone();
            foreach (var item in compound.GetAllTags().OfType<NbtString>())
            {
                if (item.Value == "@colors")
                    item.Parent[item.Name] = new NbtByteArray(map.Data.Colors);
                else if (item.Value == "@id")
                    item.Parent[item.Name] = new NbtLong(map.ID);
            }
            return compound;
        }
        NbtCompound item_maker(long id)
        {
            var compound = (NbtCompound)MapItem.Clone();
            foreach (var item in compound.GetAllTags().OfType<NbtString>())
            {
                if (item.Value == "@id")
                    item.Parent[item.Name] = new NbtLong(id);
            }
            return compound;
        }
        return new BedrockVersion(Name, data_maker, item_maker);
    }
}

public delegate NbtCompound MapMaker(Map map);
public delegate NbtCompound ItemMaker(long id);

public class BedrockVersion : IBedrockVersion
{
    private readonly MapMaker DataMaker;
    private readonly ItemMaker ItemMaker;
    public string Name { get; }
    public BedrockVersion(string name, MapMaker data, ItemMaker item)
    {
        Name = name;
        DataMaker = data;
        ItemMaker = item;
    }

    public NbtCompound CreateMapCompound(Map map) => DataMaker(map);
    public NbtCompound CreateMapItem(long id) => ItemMaker(id);
}
