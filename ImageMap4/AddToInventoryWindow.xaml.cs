using System.Windows;
using System.Windows.Input;

namespace ImageMap4;
/// <summary>
/// Interaction logic for class AddToInventory.xaml
/// </summary>
public partial class AddToInventoryWindow : Window
{
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public MainViewModel Parent { get; }

    public IInventory SelectedInventory
    {
        get
        {
            if (Properties.Settings.Default.InventoryChoice >= Parent.PlayerList.Count || Properties.Settings.Default.InventoryChoice < 0)
                Properties.Settings.Default.InventoryChoice = 1;
            return Parent.PlayerList[Properties.Settings.Default.InventoryChoice];
        }
        set { Properties.Settings.Default.InventoryChoice = Parent.PlayerList.IndexOf(value); }
    }

    public AddToInventoryWindow(MainViewModel parent)
    {
        Parent = parent;
        InitializeComponent();
        ConfirmCommand = new RelayCommand(() =>
        {
            DialogResult = true;
            this.Close();
        });
        CancelCommand = new RelayCommand(() =>
        {
            DialogResult = false;
            this.Close();
        });
    }
}

