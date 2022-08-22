using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for ChangeIDWindow.xaml
/// </summary>
public partial class ChangeIDWindow : Window, INotifyPropertyChanged
{
    public ChangeResult Result;
    private long _id;
    public long ID
    {
        get { return _id; }
        set
        {
            _id = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
            bool conflicts = false;
            for (long i = _id; i < _id + Count; i++)
            {
                if (TakenIDs.Contains(i))
                {
                    conflicts = true;
                    break;
                }
            }
            Conflicts = conflicts;
        }
    }
    private bool _conflicts;
    public bool Conflicts
    {
        get { return _conflicts; }
        set { _conflicts = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conflicts))); }
    }
    public ICommand ConfirmCommand { get; }
    public ICommand AutoCommand { get; }
    public ICommand CancelCommand { get; }
    private readonly HashSet<long> TakenIDs;
    private readonly int Count;
    public event PropertyChangedEventHandler? PropertyChanged;

    public ChangeIDWindow(long starting, int count, HashSet<long> taken)
    {
        TakenIDs = taken;
        ID = starting;
        Count = count;
        InitializeComponent();
        ConfirmCommand = new RelayCommand(() =>
        {
            Result = ChangeResult.Confirmed;
            DialogResult = true;
            this.Close();
        });
        AutoCommand = new RelayCommand(() =>
        {
            Result = ChangeResult.Auto;
            DialogResult = true;
            this.Close();
        });
        CancelCommand = new RelayCommand(() =>
        {
            Result = ChangeResult.Cancelled;
            this.Close();
        });
        Input.Focus();
    }
}

public enum ChangeResult
{
    Confirmed,
    Auto,
    Cancelled
}
