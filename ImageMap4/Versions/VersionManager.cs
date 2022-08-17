using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImageMap4;

public class VersionManager
{
    private static readonly List<Update> Updates;
    static VersionManager()
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new YamlColorConverter())
            .WithTypeConverter(new YamlNbtConverter())
            .Build();
        Updates = deserializer.Deserialize<List<Update>>(Properties.Resources.versions);
    }

    public static IJavaVersion DetermineVersion(NbtCompound leveldat)
    {
        var version = new JavaVersionBuilder();
        int last = Updates.FindLastIndex(x => x.Check.Passes(leveldat));
        for (int i = 0; i <= last; i++)
        {
            version.Add(Updates[i]);
        }
        return version.Build();
    }
}

internal class YamlColorConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(Rgba32);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        var scalar = parser.Consume<Scalar>();
        Color.TryParse(scalar.Value, out var color);
        return color;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        throw new NotSupportedException();
    }
}

internal class YamlNbtConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(NbtTag).IsAssignableFrom(type);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        if (parser.TryConsume<MappingStart>(out var _))
        {
            var compound = new NbtCompound();
            while (!parser.TryConsume<MappingEnd>(out var _))
            {
                var key = parser.Consume<Scalar>().Value;
                var value = (NbtTag)ReadYaml(parser, typeof(NbtTag));
                value.Name = key;
                compound.Add(value);
            }
            return compound;
        }
        if (parser.TryConsume<SequenceStart>(out var _))
        {
            var list = new NbtList();
            while (!parser.TryConsume<SequenceEnd>(out var _))
            {
                var value = (NbtTag)ReadYaml(parser, typeof(NbtTag));
                list.Add(value);
            }
            return list;
        }
        else
        {
            var scalar = parser.Consume<Scalar>();
            if (SnbtParser.ClassicTryParse(scalar.Value, false, out var result))
                return result;
            return new NbtString(scalar.Value);
        }
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        throw new NotSupportedException();
    }
}

public class Update
{
    public VersionCheck Check;
    public byte[]? Multipliers;
    public List<Rgba32>? SetBaseColors;
    public List<Rgba32>? AddBaseColors;
    public NbtCompound? MapItem;
    public NbtCompound? MapData;
    public NbtCompound? MapEntity;
    public bool? StructuresSupported;
    public string? StructureFolder;
    public string Name;
}

public class VersionCheck
{
    public string[]? Path;
    public int? DataVersion;
    public bool Passes(NbtCompound leveldat)
    {
        if (DataVersion.HasValue)
        {
            var dataversion = leveldat.Get<NbtInt>("DataVersion");
            if (dataversion != null)
                return dataversion.Value >= DataVersion.Value;
        }
        if (Path != null)
        {
            NbtTag? tag = leveldat;
            foreach (var item in Path)
            {
                tag = leveldat?[item];
            }
            return tag != null;
        }
        return false;
    }
}
