using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageMap4;
public class ImageViewModel : ObservableObject
{
    public GridMakerViewModel GridMaker { get; }
    public MainViewModel Parent => GridMaker.Parent;

    public ImageViewModel(GridMakerViewModel child)
    {
        GridMaker = child;
    }

    public Image<Rgba32> CreateImage()
    {
        var output = new Image<Rgba32>(128 * GridMaker.GridWidth, 128 * GridMaker.GridHeight);
        output.Mutate(o =>
        {
            for (int y = 0; y < GridMaker.GridHeight; y++)
            {
                for (int x = 0; x < GridMaker.GridWidth; x++)
                {
                    var img = GridMaker.Grid[x, y];
                    if (img != null)
                        o.DrawImage(img.Data.Image, new Point(x * 128, y * 128), 1f);
                }
            }
        });
        return output;
    }
}
