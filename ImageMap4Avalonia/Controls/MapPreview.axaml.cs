using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageMap4;

public partial class MapPreview : UserControl
{
    public MapPreview()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}