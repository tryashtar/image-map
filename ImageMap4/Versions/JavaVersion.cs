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
    bool StructuresSupported { get; }
    NbtCompound CreateStructureFile(long?[,] mapids);
    string StructureFileLocation(string world_folder, string identifier);
}

public class JavaVersionBuilder
{
    public readonly List<Rgba32> BaseColors = new();
    public byte[]? Multipliers;
    public NbtCompound? MapEntity;
    public NbtCompound? MapData;
    public NbtCompound? MapItem;
    public bool StructuresSupported = false;
    public string? StructureFolder;
    public string? Name;
    public void Add(JavaUpdate update)
    {
        if (update.SetBaseColors != null)
        {
            this.BaseColors.Clear();
            this.BaseColors.AddRange(update.SetBaseColors);
        }
        if (update.AddBaseColors != null)
            this.BaseColors.AddRange(update.AddBaseColors);
        this.Multipliers = update.Multipliers ?? this.Multipliers;
        this.MapEntity = update.MapEntity ?? this.MapEntity;
        this.MapData = update.MapData ?? this.MapData;
        this.MapItem = update.MapItem ?? this.MapItem;
        this.Name = update.Name ?? this.Name;
        this.StructureFolder = update.StructureFolder ?? this.StructureFolder;
        this.StructuresSupported |= update.StructuresSupported ?? false;
    }
    public IJavaVersion Build()
    {
        NbtCompound item_maker(long id)
        {
            var compound = (NbtCompound)MapItem.Clone();
            foreach (var item in compound.GetAllTags().OfType<NbtString>())
            {
                if (item.Value == "@s")
                    item.Parent[item.Name] = new NbtShort((short)id);
                else if (item.Value == "@i")
                    item.Parent[item.Name] = new NbtInt((int)id);
            }
            return compound;
        }
        NbtCompound frame_maker()
        {
            return (NbtCompound)MapEntity.Clone();
        }
        NbtCompound data_maker(MapData data)
        {
            var compound = (NbtCompound)MapData.Clone();
            foreach (var item in compound.GetAllTags().OfType<NbtString>())
            {
                if (item.Value == "@colors")
                    item.Parent[item.Name] = new NbtByteArray(data.Colors);
            }
            return compound;
        }
        string structure_folder(string world, string identifier)
        {
            if (StructureFolder == "structures")
                return Path.Combine(world, "structures", identifier.Replace(':', '_') + ".nbt");
            int colon = identifier.IndexOf(':');
            return Path.Combine(world, StructureFolder, identifier[..colon], identifier[(colon + 1)..] + ".nbt");
        }
        return new JavaVersion(Name, GetPalette(), item_maker, frame_maker, data_maker, structure_folder, StructuresSupported);
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

public delegate NbtCompound MapDataMaker(MapData data);
public delegate NbtCompound MapItemMaker(long id);
public delegate NbtCompound ItemFrameMaker();
public delegate string StructurePathGetter(string world, string identifier);

public class JavaVersion : IJavaVersion
{
    private readonly Color[] Palette;
    private readonly Dictionary<byte, Rgba32> ColorMap = new();
    private readonly Dictionary<Color, byte> ReverseColorMap = new();
    private readonly MapItemMaker MapMaker;
    private readonly ItemFrameMaker FrameMaker;
    private readonly MapDataMaker DataMaker;
    private readonly StructurePathGetter StructurePath;
    public string Name { get; }
    public bool StructuresSupported { get; }
    public JavaVersion(string name, IEnumerable<Color> palette, MapItemMaker maps, ItemFrameMaker frames, MapDataMaker data, StructurePathGetter path, bool structures)
    {
        Name = name;
        MapMaker = maps;
        FrameMaker = frames;
        DataMaker = data;
        StructurePath = path;
        StructuresSupported = structures;
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

    public NbtCompound CreateStructureFile(long?[,] mapids)
    {
        var entities = new NbtList();
        for (int y = 0; y < mapids.GetLength(0); y++)
        {
            for (int x = 0; x < mapids.GetLength(1); x++)
            {
                long? val = mapids[y, x];
                if (val.HasValue)
                {
                    var frame = FrameMaker();
                    frame.Name = "nbt";
                    var item = MapMaker(val.Value);
                    item.Name = "Item";
                    frame.Add(item);
                    entities.Add(new NbtCompound()
                    {
                        frame,
                        new NbtList("blockPos") { new NbtInt(0), new NbtInt(y), new NbtInt(x) }
                    });
                }
            }
        }
        return new NbtCompound() {
            new NbtList("size") {
                new NbtInt(1), new NbtInt(mapids.GetLength(0)), new NbtInt(mapids.GetLength(1))
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

    public NbtCompound CreateMapCompound(MapData map) => DataMaker(map);
    public NbtCompound MakeMapItem(long id) => MapMaker(id);
    public string StructureFileLocation(string world_folder, string identifier) => StructurePath(world_folder, identifier);
}
