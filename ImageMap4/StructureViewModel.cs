﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public Map[,] Grid { get; private set; } = new Map[1, 1];

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
        set { _parent = value; OnPropertyChanged(); }
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
