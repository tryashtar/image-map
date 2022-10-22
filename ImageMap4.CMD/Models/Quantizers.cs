using Colourful;
using fNbt;
using LevelDB;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
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

// Based on ImageSharp's PaletteQuantizer
// allows you to inject your own pixel map, so you can use a distance algorithm besides euclidean
public class CustomQuantizer : IQuantizer
{
    private readonly ReadOnlyMemory<Color> colorPalette;
    private readonly IColorAlgorithm colorAlgorithm;
    public QuantizerOptions Options { get; }
    public CustomQuantizer(QuantizerOptions options, ReadOnlyMemory<Color> palette, IColorAlgorithm algorithm)
    {
        this.Options = options;
        this.colorPalette = palette;
        this.colorAlgorithm = algorithm;
    }

    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration)
            where TPixel : unmanaged, IPixel<TPixel>
            => this.CreatePixelSpecificQuantizer<TPixel>(configuration, this.Options);

    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration, QuantizerOptions options)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        int length = Math.Min(this.colorPalette.Length, options.MaxColors);
        var palette = new TPixel[length];
        Color.ToPixel(configuration, this.colorPalette.Span, palette.AsSpan());
        return new CustomQuantizer<TPixel>(configuration, options, new PixelMap<TPixel>(configuration, palette, colorAlgorithm));
    }
}

public struct CustomQuantizer<TPixel> : IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
    private PixelMap<TPixel> PixelMap;
    public CustomQuantizer(Configuration configuration, QuantizerOptions options, PixelMap<TPixel> map)
    {
        this.Configuration = configuration;
        this.Options = options;
        this.PixelMap = map;
    }

    public Configuration Configuration { get; }
    public QuantizerOptions Options { get; }
    public ReadOnlyMemory<TPixel> Palette => PixelMap.Palette;

    public readonly IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
        => QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(this), source, bounds);

    public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion)
    {
    }

    public readonly byte GetQuantizedColor(TPixel color, out TPixel match)
        => (byte)this.PixelMap.GetClosestColor(color, out match);

    public void Dispose()
    {
        this.PixelMap?.Dispose();
        this.PixelMap = null!;
    }
}

// Based on ImageSharp's (internal) EuclideanPixelMap
// makes DistanceSquared abstract so it can be any algorithm
public class PixelMap<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
    private ColorDistanceCache cache;
    private readonly Rgba32[] rgbaPalette;
    private readonly IColorAlgorithm algorithm;
    public ReadOnlyMemory<TPixel> Palette { get; }
    public PixelMap(Configuration configuration, ReadOnlyMemory<TPixel> palette, IColorAlgorithm algorithm)
    {
        this.algorithm = algorithm;
        this.Palette = palette;
        this.rgbaPalette = new Rgba32[palette.Length];
        this.cache = new ColorDistanceCache(configuration.MemoryAllocator);
        PixelOperations<TPixel>.Instance.ToRgba32(configuration, this.Palette.Span, this.rgbaPalette);
    }

    public int GetClosestColor(TPixel color, out TPixel match)
    {
        ref TPixel paletteRef = ref MemoryMarshal.GetReference(this.Palette.Span);
        Unsafe.SkipInit(out Rgba32 rgba);
        color.ToRgba32(ref rgba);
        if (!this.cache.TryGetValue(rgba, out short index))
            return this.GetClosestColorSlow(rgba, ref paletteRef, out match);
        match = Unsafe.Add(ref paletteRef, index);
        return index;
    }

    private int GetClosestColorSlow(Rgba32 rgba, ref TPixel paletteRef, out TPixel match)
    {
        int index = 0;
        double leastDistance = double.MaxValue;
        for (int i = 0; i < this.rgbaPalette.Length; i++)
        {
            Rgba32 candidate = this.rgbaPalette[i];
            double distance = algorithm.Distance(rgba, candidate);

            // custom bit here: transparency is special
            distance += 10000 * Math.Abs(rgba.A - candidate.A);

            if (distance == 0)
            {
                index = i;
                break;
            }
            if (distance < leastDistance)
            {
                index = i;
                leastDistance = distance;
            }
        }
        this.cache.Add(rgba, (byte)index);
        match = Unsafe.Add(ref paletteRef, index);
        return index;
    }

    public void Dispose()
    {
        this.cache.Dispose();
    }

    private unsafe struct ColorDistanceCache : IDisposable
    {
        private const int IndexBits = 5;
        private const int IndexAlphaBits = 5;
        private const int IndexCount = (1 << IndexBits) + 1;
        private const int IndexAlphaCount = (1 << IndexAlphaBits) + 1;
        private const int RgbShift = 8 - IndexBits;
        private const int AlphaShift = 8 - IndexAlphaBits;
        private const int Entries = IndexCount * IndexCount * IndexCount * IndexAlphaCount;
        private MemoryHandle tableHandle;
        private readonly IMemoryOwner<short> table;
        private readonly short* tablePointer;

        public ColorDistanceCache(MemoryAllocator allocator)
        {
            this.table = allocator.Allocate<short>(Entries);
            this.table.Memory.Span.Fill(-1);
            this.tableHandle = this.table.Memory.Pin();
            this.tablePointer = (short*)this.tableHandle.Pointer;
        }

        public void Add(Rgba32 rgba, byte index)
        {
            int r = rgba.R >> RgbShift;
            int g = rgba.G >> RgbShift;
            int b = rgba.B >> RgbShift;
            int a = rgba.A >> AlphaShift;
            int idx = GetPaletteIndex(r, g, b, a);
            this.tablePointer[idx] = index;
        }

        public bool TryGetValue(Rgba32 rgba, out short match)
        {
            int r = rgba.R >> RgbShift;
            int g = rgba.G >> RgbShift;
            int b = rgba.B >> RgbShift;
            int a = rgba.A >> AlphaShift;
            int idx = GetPaletteIndex(r, g, b, a);
            match = this.tablePointer[idx];
            return match > -1;
        }

        public void Clear() => this.table.Memory.Span.Fill(-1);

        private static int GetPaletteIndex(int r, int g, int b, int a)
            => (r << ((IndexBits << 1) + IndexAlphaBits))
            + (r << (IndexBits + IndexAlphaBits + 1))
            + (g << (IndexBits + IndexAlphaBits))
            + (r << (IndexBits << 1))
            + (r << (IndexBits + 1))
            + (g << IndexBits)
            + ((r + g + b) << IndexAlphaBits)
            + r + g + b + a;

        public void Dispose()
        {
            if (this.table != null)
            {
                this.tableHandle.Dispose();
                this.table.Dispose();
            }
        }
    }
}
