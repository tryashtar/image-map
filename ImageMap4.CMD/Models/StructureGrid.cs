using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;

public class StructureGrid
{
    public string Identifier { get; }
    public Map?[,] Tiles { get; }
    public int GridHeight => Tiles.GetLength(1);
    public int GridWidth => Tiles.GetLength(0);
    public bool GlowingFrames { get; init; }
    public bool InvisibleFrames { get; init; }
    public StructureGrid(string identifier, Map?[,] tiles)
    {
        Identifier = MakeSafeIdentifier(identifier);
        Tiles = tiles;
    }
    public StructureGrid(Map?[,] tiles)
    {
        Tiles = tiles;
        // generate name automatically from size and map IDs
        var name = new StringBuilder("imagemap:");
        name.Append(GridWidth);
        name.Append('x');
        name.Append(GridHeight);
        name.Append('_');
        long? first_id = null;
        long? last_id = null;
        foreach (var map in tiles)
        {
            if (map != null)
            {
                first_id ??= map.ID;
                last_id = map.ID;
            }
        }
        name.Append(first_id);
        name.Append('-');
        name.Append(last_id);
        Identifier = name.ToString();
    }
    public static string MakeSafeIdentifier(string input)
    {
        input = Path.GetFileNameWithoutExtension(input);
        input = input.ToLower();
        input = input.Replace(' ', '_');
        return input;
    }
    public long?[,] ToIDGrid()
    {
        long?[,] grid = new long?[GridWidth, GridHeight];
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                grid[x, y] = Tiles[x, y]?.ID;
            }
        }
        return grid;
    }
}
