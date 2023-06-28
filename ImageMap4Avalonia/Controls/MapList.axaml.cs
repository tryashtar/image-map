using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageMap4;

public partial class MapList : UserControl
{
    public MapList()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}