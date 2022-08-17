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

public class BedrockVersionBuilder
{
    public string? Name;
    public NbtCompound? MapData;
    public void Add(BedrockUpdate update)
    {
        this.Name = update.Name ?? this.Name;
        this.MapData = update.MapData ?? this.MapData;
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
        return new BedrockVersion(Name, data_maker);
    }
}

public delegate NbtCompound MapMaker(Map map);

public class BedrockVersion : IBedrockVersion
{
    private readonly MapMaker DataMaker;
    public string Name { get; }
    public BedrockVersion(string name,  MapMaker data)
    {
        Name = name;
        DataMaker = data;
    }

    public NbtCompound CreateMapCompound(Map map) => DataMaker(map);
}
