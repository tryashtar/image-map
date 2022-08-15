using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageMap4;

// https://github.com/jongleur1983/SharpImageSource
public sealed class ImageSharpImageSource<TPixel> : BitmapSource
    where TPixel : unmanaged, IPixel<TPixel>
{
    private Image<TPixel> source;

    protected override Freezable CreateInstanceCore()
    {
        return new ImageSharpImageSource<TPixel>();
    }

    public ImageSharpImageSource()
        : this(100, 100)
    {
    }

    public ImageSharpImageSource(
        int width,
        int height)
    {
        this.source = new Image<TPixel>(width, height);
    }

    public ImageSharpImageSource(
        Image<TPixel> source)
    {
        this.source = source;
    }

    private void SetPixel(
        ref byte[] bits,
        int x,
        int y,
        int stride,
        System.Windows.Media.Color c)
    {
        bits[x * 3 + y * stride] = c.R;
        bits[x * 3 + y * stride + 1] = c.G;
        bits[x * 3 + y * stride + 2] = c.B;
    }

    public override PixelFormat Format => PixelFormats.Bgra32;

    public override int PixelHeight => this.source.Height;

    public override int PixelWidth => this.source.Width;

    public override double DpiX => this.source.Metadata.HorizontalResolution;

    public override double DpiY => this.source.Metadata.VerticalResolution;

    public override BitmapPalette Palette => null;

    public override event EventHandler<ExceptionEventArgs> DecodeFailed;

    public override void CopyPixels(Array pixels, int stride, int offset)
    {
        Int32Rect sourceRect = new Int32Rect(0, 0, this.PixelWidth, this.PixelHeight);
        base.CopyPixels(sourceRect, pixels, stride, offset);
    }

    public override void CopyPixels(
        Int32Rect sourceRect,
        Array pixels,
        int stride,
        int offset)
    {
        this.ValidateArrayAndGetInfo(
            pixels,
            out var elementSize,
            out var bufferSize,
            out var elementType);

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        // We accept arrays of arbitrary value types - but not reference types.
        if (elementType == null || !elementType.IsValueType)
        {
            throw new ArgumentException("must be a valueType!", nameof(pixels));
        }

        checked
        {
            int offsetInBytes = offset * elementSize;
            if (offsetInBytes >= bufferSize)
            {
                throw new IndexOutOfRangeException();
            }

            // Get the address of the data in the array by pinning it.
            GCHandle arrayHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                // Adjust the buffer and bufferSize to account for the offset.
                IntPtr buffer = arrayHandle.AddrOfPinnedObject();
                buffer = new IntPtr(((long)buffer) + (long)offsetInBytes);
                bufferSize -= offsetInBytes;

                CopyPixels(sourceRect, buffer, bufferSize, stride);
            }
            finally
            {
                arrayHandle.Free();
            }
        }
    }

    public override void CopyPixels(
        Int32Rect sourceRect,
        IntPtr buffer,
        int bufferSize,
        int stride)
    {
        // WIC would specify NULL for the source rect to indicate that the
        // entire content should be copied.  WPF turns that into an empty
        // rect, which we inflate here to be the entire bounds.
        if (sourceRect.IsEmpty)
        {
            sourceRect.Width = PixelWidth;
            sourceRect.Height = PixelHeight;
        }

        if (sourceRect.X < 0
            || sourceRect.Width < 0
            || sourceRect.Y < 0
            || sourceRect.Height < 0
            || sourceRect.X + sourceRect.Width > PixelWidth
            || sourceRect.Y + sourceRect.Height > PixelHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceRect));
        }

        if (buffer == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        checked
        {
            if (stride < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stride));
            }

            uint minStrideInBits = (uint)(sourceRect.Width * Format.BitsPerPixel);
            uint minStrideInBytes = ((minStrideInBits + 7) / 8);
            if (stride < minStrideInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(stride));
            }

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            uint minBufferSize = (uint)((sourceRect.Height - 1) * stride) + minStrideInBytes;
            if (bufferSize < minBufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }
        }

        CopyPixelsCore(sourceRect, stride, bufferSize, buffer);
    }

    private void CopyPixelsCore(
        Int32Rect sourceRect,
        int stride,
        int bufferSize,
        IntPtr buffer)
    {
        if (this.source != null)
        {
            unsafe
            {
                byte* pBytes = (byte*)buffer.ToPointer();
                for (int y = 0; y < sourceRect.Height; y++)
                {
                    Bgra32* pPixel = (Bgra32*)pBytes;

                    for (int x = 0; x < sourceRect.Width; x++)
                    {
                        Rgba32 dest = default(Rgba32);
                        this.source[x, y].ToRgba32(ref dest);

                        // Write sRGB (non-linear) since it is implied by
                        // the pixel format we chose.
                        pPixel->G = dest.G;
                        pPixel->R = dest.R;
                        pPixel->B = dest.B;
                        pPixel->A = dest.A;

                        pPixel++;
                    }

                    pBytes += stride;
                }
            }
        }
    }

    private void ValidateArrayAndGetInfo(
        Array pixels,
        out int elementSize,
        out int sourceBufferSize,
        out Type elementType)
    {
        if (pixels == null)
        {
            throw new ArgumentNullException(nameof(pixels));
        }

        if (pixels.Rank == 1)
        {
            if (pixels.GetLength(0) <= 0)
            {
                throw new ArgumentException(nameof(pixels));
            }
            else
            {
                checked
                {
                    object exemplar = pixels.GetValue(0);
                    elementSize = Marshal.SizeOf(exemplar);
                    sourceBufferSize = pixels.GetLength(0) * elementSize;
                    elementType = exemplar.GetType();
                }
            }
        }
        else if (pixels.Rank == 2)
        {
            if (pixels.GetLength(0) <= 0 || pixels.GetLength(1) <= 0)
            {
                throw new ArgumentException(nameof(pixels));
            }
            else
            {
                checked
                {
                    object exemplar = pixels.GetValue(0, 0);
                    elementSize = Marshal.SizeOf(exemplar);
                    sourceBufferSize = pixels.GetLength(0) * pixels.GetLength(1) * elementSize;
                    elementType = exemplar.GetType();
                }
            }
        }
        else
        {
            throw new ArgumentException(nameof(pixels));
        }
    }

    public override bool IsDownloading => false;

    public override event EventHandler DownloadCompleted;

    public override event EventHandler<DownloadProgressEventArgs> DownloadProgress;

    public override event EventHandler<ExceptionEventArgs> DownloadFailed;
}
