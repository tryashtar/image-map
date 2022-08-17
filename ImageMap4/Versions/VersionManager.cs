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
    private static readonly List<(JavaVersionCheck check, IJavaVersion version)> JavaVersions = new();
    private static readonly List<(BedrockVersionCheck check, IBedrockVersion version)> BedrockVersions = new();
    static VersionManager()
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new YamlColorConverter())
            .WithTypeConverter(new YamlNbtConverter())
            .Build();
        var updates = deserializer.Deserialize<UpdateLists>(Properties.Resources.versions);
        var java = new JavaVersionBuilder();
        foreach (var update in updates.Java)
        {
            java.Add(update);
            JavaVersions.Add((update.Check, java.Build()));
        }
        var bedrock = new BedrockVersionBuilder();
        foreach (var update in updates.Bedrock)
        {
            bedrock.Add(update);
            BedrockVersions.Add((update.Check, bedrock.Build()));
        }
    }

    public static IJavaVersion DetermineJavaVersion(NbtCompound leveldat)
    {
        return JavaVersions.Last(x => x.check.Passes(leveldat)).version;
    }

    public static IBedrockVersion DetermineBedrockVersion(NbtCompound leveldat)
    {
        return BedrockVersions.Last(x => x.check.Passes(leveldat)).version;
    }
}

internal class UpdateLists
{
    public List<JavaUpdate> Java;
    public List<BedrockUpdate> Bedrock;
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

    public object ReadYaml(IParser parser, Type type)
    {
        if (parser.TryConsume<MappingStart>(out _))
        {
            var compound = new NbtCompound();
            while (!parser.TryConsume<MappingEnd>(out _))
            {
                var key = parser.Consume<Scalar>().Value;
                var value = (NbtTag)ReadYaml(parser, typeof(NbtTag));
                value.Name = key;
                compound.Add(value);
            }
            return compound;
        }
        if (parser.TryConsume<SequenceStart>(out _))
        {
            var list = new NbtList();
            while (!parser.TryConsume<SequenceEnd>(out _))
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

public class JavaUpdate
{
    public string Name;
    public JavaVersionCheck Check;
    public byte[]? Multipliers;
    public List<Rgba32>? SetBaseColors;
    public List<Rgba32>? AddBaseColors;
    public NbtCompound? MapItem;
    public NbtCompound? MapData;
    public NbtCompound? MapEntity;
    public bool? StructuresSupported;
    public string? StructureFolder;
}

public class BedrockUpdate
{
    public string Name;
    public BedrockVersionCheck Check;
    public NbtCompound? MapData;
}

public class BedrockVersionCheck
{
    public int[] Version;
    public bool Passes(NbtCompound leveldat)
    {
        var versiontag = leveldat.Get<NbtList>("lastOpenedWithVersion");
        if (versiontag != null)
        {
            for (int i = 0; i < Math.Min(Version.Length, versiontag.Count); i++)
            {
                if (versiontag[i].IntValue < Version[i])
                    return false;
            }
            return true;
        }
        return false;
    }
}

public class JavaVersionCheck
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
