using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;

public interface IJavaVersion
{
    ReadOnlyMemory<Color> GetPalette();
    Image<Rgba32> Decode(byte[] colors);
    byte[] EncodeColors(Image<Rgba32> image);
    NbtCompound CreateMapCompound(MapData map);
    NbtCompound MakeMapItem(long id);
    NbtCompound MakeStructureItem(StructureGrid structure);
    bool StructuresSupported { get; }
    NbtCompound CreateStructureFile(StructureGrid structure);
    string StructureFileLocation(string world_folder, string identifier);
    int? DataVersion { get; }
}

public class JavaVersionBuilder
{
    public readonly List<Rgba32> BaseColors = new();
    public byte[]? Multipliers;
    public NbtTemplate? MapEntity;
    public NbtTemplate? MapData;
    public NbtTemplate? MapItem;
    public NbtTemplate? StructureItem;
    public bool StructuresSupported = false;
    public string? StructureFolder;
    public string? Name;
    public int? DataVersion;
    public void Add(JavaUpdate update, int? data_version)
    {
        if (update.SetBaseColors != null)
        {
            this.BaseColors.Clear();
            this.BaseColors.AddRange(update.SetBaseColors);
        }
        if (update.AddBaseColors != null)
            this.BaseColors.AddRange(update.AddBaseColors);
        this.Multipliers = update.Multipliers ?? this.Multipliers;
        if (update.MapEntity != null)
            this.MapEntity = new(update.MapEntity);
        if (update.MapData != null)
            this.MapData = new(update.MapData);
        if (update.MapItem != null)
            this.MapItem = new(update.MapItem);
        if (update.StructureItem != null)
            this.StructureItem = new(update.StructureItem);
        this.Name = update.Name ?? this.Name;
        this.StructureFolder = update.StructureFolder ?? this.StructureFolder;
        this.StructuresSupported |= update.StructuresSupported ?? false;
        if (data_version != null)
            this.DataVersion = data_version;
    }
    public IJavaVersion Build()
    {
        return new JavaVersion(Name, GetPalette())
        {
            MapMaker = MapItem,
            FrameMaker = MapEntity,
            DataMaker = MapData,
            StructureMaker = StructureItem,
            StructureFolder = StructureFolder,
            StructuresSupported = StructuresSupported,
            DataVersion = DataVersion
        };
    }

    private IEnumerable<Color> GetPalette()
    {
        foreach (var color in BaseColors)
        {
            var alts = GetAlternateColors(color);
            foreach (var alt in alts)
            {
                yield return alt;
            }
        }
    }

    private IEnumerable<Rgba32> GetAlternateColors(Rgba32 color)
    {
        return Multipliers.Select(x => Multiply(color, x));
    }

    private static Rgba32 Multiply(Rgba32 color, byte value)
    {
        return new Rgba32((byte)(color.R * value / 255), (byte)(color.G * value / 255), (byte)(color.B * value / 255), color.A);
    }
}

public class JavaVersion : IJavaVersion
{
    private readonly Color[] Palette;
    private readonly Dictionary<byte, Rgba32> ColorMap = new();
    private readonly Dictionary<Color, byte> ReverseColorMap = new();
    public NbtTemplate MapMaker { get; init; }
    public NbtTemplate StructureMaker { get; init; }
    public NbtTemplate FrameMaker { get; init; }
    public NbtTemplate DataMaker { get; init; }
    public string StructureFolder { get; init; }
    public string Name { get; init; }
    public bool StructuresSupported { get; init; }
    public int? DataVersion { get; init; }
    public JavaVersion(string name, IEnumerable<Color> palette)
    {
        Name = name;
        Palette = palette.ToArray();
        for (byte i = 0; i < Palette.Length; i++)
        {
            ColorMap[i] = Palette[i];
            if (!ReverseColorMap.ContainsKey(Palette[i]))
                ReverseColorMap[Palette[i]] = i;
        }
    }

    public ReadOnlyMemory<Color> GetPalette() => Palette;

    public Image<Rgba32> Decode(byte[] colors)
    {
        byte[] pixels = new byte[128 * 128 * 4];
        for (int i = 0; i < colors.Length; i++)
        {
            if (ColorMap.TryGetValue(colors[i], out var color))
            {
                pixels[4 * i] = color.R;
                pixels[4 * i + 1] = color.G;
                pixels[4 * i + 2] = color.B;
                pixels[4 * i + 3] = color.A;
            }
        }
        return Image.LoadPixelData<Rgba32>(pixels, 128, 128);
    }

    public byte[] EncodeColors(Image<Rgba32> image)
    {
        byte[] result = new byte[image.Width * image.Height];
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var b = image[x, y];
                result[y * image.Height + x] = ReverseColorMap[b];
            }
        }
        return result;
    }

    public NbtCompound CreateStructureFile(StructureGrid structure)
    {
        var mapids = structure.ToIDGrid();
        var entities = new NbtList("entities");
        for (int y = 0; y < structure.GridHeight; y++)
        {
            for (int x = 0; x < structure.GridWidth; x++)
            {
                long? val = mapids[x, y];
                if (val.HasValue)
                {
                    var frame = FrameMaker.Create(
                         ("id", () => new NbtString(structure.GlowingFrames ? "minecraft:glow_item_frame" : "minecraft:item_frame")),
                         ("invisible", () => new NbtByte(structure.InvisibleFrames))
                     );
                    frame.Name = "nbt";
                    var item = MakeMapItem(val.Value);
                    item.Name = "Item";
                    frame.Add(item);
                    entities.Add(new NbtCompound()
                    {
                        frame,
                        new NbtList("blockPos") { new NbtInt(0), new NbtInt(structure.GridHeight - y - 1), new NbtInt(x) },
                        new NbtList("pos") { new NbtDouble(0), new NbtDouble(structure.GridHeight - y - 1 + 0.5), new NbtDouble(x + 0.5) }
                    });
                }
            }
        }
        return new NbtCompound("") {
            new NbtList("size") {
                new NbtInt(1), new NbtInt(structure.GridHeight), new NbtInt(structure.GridWidth)
            },
            entities,
            new NbtList("blocks") {
                new NbtCompound() {
                    new NbtList("pos") { new NbtInt(0), new NbtInt(0), new NbtInt(0) },
                    new NbtInt("state", 0)
                }
            },
            new NbtList("palette") {
                new NbtCompound() {
                    new NbtString("Name", "minecraft:air")
                }
            }
        };
    }

    public NbtCompound CreateMapCompound(MapData map) => DataMaker.Create(("colors", () => new NbtByteArray(map.Colors)));
    public NbtCompound MakeMapItem(long id) => MapMaker.Create(
        ("s", () => new NbtShort((short)id)),
        ("i", () => new NbtInt((int)id))
    );
    public NbtCompound MakeStructureItem(StructureGrid structure)
    {
        string identifier = structure.Identifier;
        if (StructureFolder == "structures")
            identifier = identifier.Replace(':', '_');
        return StructureMaker.Create(
            ("id", () => new NbtString(structure.Identifier)),
            ("old_name", () => new NbtString($"§r§d{structure.Identifier}§r")),
            ("name", () => new NbtString($"{{\"text\":\"{structure.Identifier}\",\"italic\":false}}")),
            ("x", () => new NbtInt(1)),
            ("y", () => new NbtInt(structure.GridHeight)),
            ("z", () => new NbtInt(structure.GridWidth))
        );
    }
    public string StructureFileLocation(string world_folder, string identifier)
    {
        if (StructureFolder == "structures")
            return Path.Combine(world_folder, "structures", identifier.Replace(':', '_') + ".nbt");
        int colon = identifier.IndexOf(':');
        return Path.Combine(world_folder, StructureFolder, identifier[..colon], "structures", identifier[(colon + 1)..] + ".nbt");
    }
}
