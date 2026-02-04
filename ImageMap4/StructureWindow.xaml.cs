using System.Windows;

namespace ImageMap4;
/// <summary>
/// Interaction logic for StructureWindow.xaml
/// </summary>
public partial class StructureWindow : Window
{
    public StructureViewModel ViewModel => (StructureViewModel)DataContext;
    public StructureWindow(StructureViewModel context)
    {
        InitializeComponent();
        this.DataContext = context;
        ViewModel.OnClosed += (s, e) => this.Close();
    }
}
