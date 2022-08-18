﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ImageMap4;

public abstract class World
{
    public string Folder { get; }
    public string FolderName => Path.GetFileName(Folder);
    public abstract string Name { get; }
    public abstract string WorldIcon { get; }
    public abstract DateTime AccessDate { get; }
    public World(string folder)
    {
        Folder = folder;
    }

    public abstract IAsyncEnumerable<Map> GetMapsAsync();
    public abstract void AddMaps(IEnumerable<Map> maps);
    public abstract void AddStructure(StructureGrid structure, Inventory inventory);
    public abstract IEnumerable<Inventory> GetInventories();
    protected abstract void ProcessImage(Image<Rgba32> image, ProcessSettings settings);
    protected abstract byte[] EncodeColors(Image<Rgba32> image);
    public IEnumerable<MapData> MakeMaps(ImportSettings settings)
    {
        using var image = settings.Preview.Source.Image.Value;
        image.Mutate(x =>
        {
            x.Rotate((float)settings.Preview.Rotation);
            if (settings.Preview.ScaleX == -1)
                x.Flip(FlipMode.Horizontal);
            if (settings.Preview.ScaleY == -1)
                x.Flip(FlipMode.Vertical);
            x.Resize(new ResizeOptions()
            {
                Size = new(128 * settings.Width, 128 * settings.Height),
                Sampler = settings.Sampler,
                Mode = settings.ResizeMode
            });
            // uniform scale crops to content, so we need to re-add transparency to get correct size
            x.Resize(new ResizeOptions()
            {
                Size = new(128 * settings.Width, 128 * settings.Height),
                Mode = ResizeMode.BoxPad
            });
            x.BackgroundColor(settings.BackgroundColor);
        });
        var original = Split(image, settings.Width, settings.Height);
        ProcessImage(image, settings.ProcessSettings);
        var finished = Split(image, settings.Width, settings.Height);
        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                yield return new MapData(finished[x, y], original[x, y], EncodeColors(finished[x, y]));
            }
        }
    }

    private static Image<Rgba32>[,] Split(Image<Rgba32> source, int columns, int rows)
    {
        var result = new Image<Rgba32>[columns, rows];
        int width = source.Width / columns;
        int height = source.Height / rows;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var tile = new Image<Rgba32>(width, height);
                source.ProcessPixelRows(tile, (sa, ta) =>
                {
                    for (int i = 0; i < height; i++)
                    {
                        var source = sa.GetRowSpan(height * y + i);
                        var target = ta.GetRowSpan(i);
                        source.Slice(width * x, width).CopyTo(target);
                    }
                });
                result[x, y] = tile;
            }
        }
        return result;
    }
}
