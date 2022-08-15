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
    private List<Map> FlatGrid { get; } = new();

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
                _parent.ExistingMaps.ItemChanged -= ExistingMaps_ItemChanged;
            _parent = value;
            _parent.ExistingMaps.ItemChanged += ExistingMaps_ItemChanged;
            var selected = _parent.ExistingMaps.Where(x => x.IsSelected).Select(x => x.Item).ToList();
            FlatGrid.Clear();
            FlatGrid.AddRange(selected);
            int sqrt = (int)Math.Ceiling(Math.Sqrt(selected.Count));
            for (int i = 0; i < sqrt; i++)
            {
                bool check(int val)
                {
                    int div = Math.DivRem(selected.Count, val, out int remain);
                    if (remain == 0)
                    {
                        GridWidth = sqrt + i;
                        GridHeight = div;
                        return true;
                    }
                    return false;
                }
                if (check(sqrt + i) || check(sqrt - i))
                    break;
            }
            OnPropertyChanged();
        }
    }

    private void ExistingMaps_ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Selectable<Map>.IsSelected))
        {
            var map = (Selectable<Map>)sender;
            if (map.IsSelected && !FlatGrid.Contains(map.Item))
            {
                int index = FlatGrid.IndexOf(null);
                if (index == -1)
                    FlatGrid.Add(map.Item);
                else
                    FlatGrid[index] = map.Item;
            }
            else
                FlatGrid.Remove(map.Item);
            UpdateGrid();
        }
    }

    public StructureViewModel()
    {

    }

    public void MoveMap(int from_x, int from_y, int to_x, int to_y)
    {
        if (from_x == to_x && from_y == to_y)
            return;
        int from_index = from_y * GridWidth + from_x;
        int to_index = to_y * GridWidth + to_x;
        while (FlatGrid.Count <= to_index)
        {
            FlatGrid.Add(null);
        }
        FlatGrid[to_index] = FlatGrid[from_index];
        FlatGrid[from_index] = null;
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        Map[,] newgrid = new Map[GridWidth, GridHeight];
        for (int i = 0; i < Math.Min(newgrid.Length, FlatGrid.Count); i++)
        {
            int y = Math.DivRem(i, GridWidth, out int x);
            newgrid[x, y] = FlatGrid[i];
        }
        Grid = newgrid;
        OnPropertyChanged(nameof(Grid));
    }
}
