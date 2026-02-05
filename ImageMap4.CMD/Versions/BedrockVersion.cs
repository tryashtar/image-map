using fNbt;

namespace ImageMap4;

public interface IBedrockVersion
{
    NbtCompound CreateMapItem(long id);
    NbtCompound CreateMapCompound(Map map);
    bool SupportsChests { get; }
    NbtCompound CreateChestItem(string name, NbtCompound[] items);
    bool SupportsStructures { get; }
    NbtCompound CreateStructureItem(StructureGrid structure);
    bool SupportsGlowFrames { get; }
}

public class BedrockVersionBuilder
{
    public string? Name;
    public NbtTemplate? MapData;
    public NbtTemplate? MapItem;
    public NbtTemplate? ChestItem;
    public NbtTemplate? StructureItem;
    public bool GlowFrames = false;
    public void Add(BedrockUpdate update)
    {
        this.Name = update.Name ?? this.Name;
        if (update.MapItem != null)
            this.MapItem = new(update.MapItem);
        if (update.MapData != null)
            this.MapData = new(update.MapData);
        if (update.ChestItem != null)
            this.ChestItem = new(update.ChestItem);
        if (update.StructureItem != null)
            this.StructureItem = new(update.StructureItem);
        if (update.GlowFrames != null)
            this.GlowFrames = update.GlowFrames.Value;
    }
    public IBedrockVersion Build()
    {
        var exc = new NullReferenceException();
        return new BedrockVersion(Name ?? throw exc, MapData ?? throw exc, MapItem ?? throw exc, ChestItem, StructureItem)
        {
            SupportsGlowFrames = GlowFrames
        };
    }
}

public class BedrockVersion : IBedrockVersion
{
    private readonly NbtTemplate DataMaker;
    private readonly NbtTemplate ItemMaker;
    private readonly NbtTemplate? ChestMaker;
    private readonly NbtTemplate? StructureMaker;
    public bool SupportsGlowFrames { get; init; }
    public string Name { get; }
    public BedrockVersion(string name, NbtTemplate data, NbtTemplate item, NbtTemplate? chest, NbtTemplate? structure)
    {
        Name = name;
        DataMaker = data;
        ItemMaker = item;
        ChestMaker = chest;
        StructureMaker = structure;
    }

    public NbtCompound CreateMapCompound(Map map) => DataMaker.Create(
        ("colors", () => new NbtByteArray(map.Data.Colors)),
        ("id", () => new NbtLong(map.ID))
    );
    public NbtCompound CreateMapItem(long id) => ItemMaker.Create(("id", () => new NbtLong(id)));
    public bool SupportsChests => ChestMaker != null;
    public NbtCompound CreateChestItem(string name, NbtCompound[] items) => ChestMaker.Create(
        ("name", () => new NbtString(name)),
        ("items", () => new NbtList(items))
    );
    public bool SupportsStructures => StructureMaker != null;
    public NbtCompound CreateStructureItem(StructureGrid grid)
    {
        return StructureMaker.Create(
            ("id", () => new NbtString(grid.Identifier)),
            ("x", () => new NbtInt(1)),
            ("y", () => new NbtInt(grid.GridHeight)),
            ("z", () => new NbtInt(grid.GridWidth))
        );
    }
}
