using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageMap4;
public class StructureViewModel : ObservableObject
{
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public event EventHandler OnClosed;
    public event EventHandler<ImportSettings> OnConfirmed;
    public Map?[,] Grid { get; private set; } = new Map?[1, 1];

    private int _gridWidth = 1;
    public int GridWidth
    {
        get { return _gridWidth; }
        set { _gridWidth = value; UpdateGrid(); OnPropertyChanged(); }
    }

    private int _gridHeight = 1;
    public int GridHeight
    {
        get { return _gridHeight; }
        set { _gridHeight = value; UpdateGrid(); OnPropertyChanged(); }
    }

    private MainViewModel _parent;
    public MainViewModel Parent
    {
        get { return _parent; }
        set
        {
            if (_parent != null)
                _parent.ExistingMaps.ListChanged -= ExistingMaps_ListChanged;
            _parent = value;
            _parent.ExistingMaps.ListChanged += ExistingMaps_ListChanged;
            OnPropertyChanged();
        }
    }

    private void ExistingMaps_ListChanged(object? sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor.Name == nameof(Selectable<Map>.IsSelected))
        {
            var map = Parent.ExistingMaps[e.NewIndex];
            if (map.IsSelected)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        if (Grid[x, y] == null)
                        {
                            Grid[x, y] = map.Item;
                            goto done;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        if (Grid[x, y] == map.Item)
                            Grid[x, y] = null;
                    }
                }
            }
            done:
            OnPropertyChanged(nameof(Grid));
        }
    }

    public bool ShowEmptyMaps => false;


    public StructureViewModel()
    {

    }

    private void UpdateGrid()
    {
        Map[,] newgrid = new Map[GridWidth, GridHeight];
        for (int i = 0; i < Grid.Length; i++)
        {
            newgrid[i % GridWidth, i / GridWidth] = Grid[i % Grid.GetLength(0), i / Grid.GetLength(0)];
        }
        Grid = newgrid;
        OnPropertyChanged(nameof(Grid));
    }
}
