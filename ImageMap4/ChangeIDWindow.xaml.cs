using System;
using System.Collections.Generic;
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
public partial class ChangeIDWindow : Window
{
    public ChangeResult Result;
    public long ID { get; set; }
    public ICommand ConfirmCommand { get; }
    public ICommand AutoCommand { get; }
    public ICommand CancelCommand { get; }
    public ChangeIDWindow(long starting)
    {
        InitializeComponent();
        ID = starting;
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
