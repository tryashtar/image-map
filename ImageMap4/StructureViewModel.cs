using System;
using System.Windows.Input;

namespace ImageMap4;
public class StructureViewModel : ObservableObject
{
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public event EventHandler? OnClosed;
    public event EventHandler<(StructureGrid grid, IInventory inventory)>? OnConfirmed;

    public bool GlowingFrames
    {
        get { return Properties.Settings.Default.GlowingFrames; }
        set { Properties.Settings.Default.GlowingFrames = value; OnPropertyChanged(); }
    }

    public bool InvisibleFrames
    {
        get { return Properties.Settings.Default.InvisibleFrames; }
        set { Properties.Settings.Default.InvisibleFrames = value; OnPropertyChanged(); }
    }

    public IInventory SelectedInventory
    {
        get
        {
            if (Properties.Settings.Default.InventoryChoice >= Parent.PlayerList.Count || Properties.Settings.Default.InventoryChoice < 0)
                Properties.Settings.Default.InventoryChoice = 0;
            return Parent.PlayerList[Properties.Settings.Default.InventoryChoice];
        }
        set { Properties.Settings.Default.InventoryChoice = Parent.PlayerList.IndexOf(value); OnPropertyChanged(); }
    }

    public GridMakerViewModel GridMaker { get; }
    public MainViewModel Parent => GridMaker.Parent;

    public StructureViewModel(GridMakerViewModel child)
    {
        GridMaker = child;
        ConfirmCommand = new RelayCommand(() =>
        {
            OnConfirmed?.Invoke(this, (CreateStructure(), SelectedInventory));
            OnClosed?.Invoke(this, EventArgs.Empty);
        });
        CancelCommand = new RelayCommand(() =>
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        });
    }

    public StructureGrid CreateStructure()
    {
        return new(GridMaker.Grid) { GlowingFrames = GlowingFrames, InvisibleFrames = InvisibleFrames };
    }
}
