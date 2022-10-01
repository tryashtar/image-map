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
    public NbtTemplate? MapData;
    public NbtTemplate? MapItem;
    public void Add(BedrockUpdate update)
    {
        this.Name = update.Name ?? this.Name;
        if (update.MapItem != null)
            this.MapItem = new(update.MapItem);
        if (update.MapData != null)
            this.MapData = new(update.MapData);
    }
    public IBedrockVersion Build()
    {
        return new BedrockVersion(Name, MapData, MapItem);
    }
}

public class BedrockVersion : IBedrockVersion
{
    private readonly NbtTemplate DataMaker;
    private readonly NbtTemplate ItemMaker;
    public string Name { get; }
    public BedrockVersion(string name, NbtTemplate data, NbtTemplate item)
    {
        Name = name;
        DataMaker = data;
        ItemMaker = item;
    }

    public NbtCompound CreateMapCompound(Map map) => DataMaker.Create(
        ("colors", () => new NbtByteArray(map.Data.Colors)),
        ("id", () => new NbtLong(map.ID))
    );
    public NbtCompound CreateMapItem(long id) => ItemMaker.Create(("id", () => new NbtLong(id)));
}
