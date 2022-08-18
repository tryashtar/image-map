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
    public int GridHeight => Tiles.GetLength(0);
    public int GridWidth => Tiles.GetLength(1);
    public bool GlowingFrames { get; init; }
    public bool InvisibleFrames { get; init; }
    public StructureGrid(string identifier, Map?[,] tiles)
    {
        Identifier = identifier;
        Tiles = tiles;
    }
    public StructureGrid(Map?[,] tiles)
    {
        Tiles = tiles;
        var name = new StringBuilder("imagemap:");
        name.Append(tiles.GetLength(0));
        name.Append('x');
        name.Append(tiles.GetLength(1));
        name.Append('.');
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
    public long?[,] ToIDGrid()
    {
        long?[,] grid = new long?[GridHeight, GridWidth];
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                grid[y, x] = Tiles[y, x]?.ID;
            }
        }
        return grid;
    }
}
