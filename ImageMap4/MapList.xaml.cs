using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for MapList.xaml
/// </summary>
public partial class MapList : UserControl
{
    public static readonly DependencyProperty MapsProperty =
           DependencyProperty.Register(nameof(Maps), typeof(ObservableCollection<Map>),
           typeof(MapList), new FrameworkPropertyMetadata(MapsChanged));

    public HashSet<Map> SelectedMaps { get; set; }

    public ObservableCollection<Map> Maps
    {
        get { return (ObservableCollection<Map>)GetValue(MapsProperty); }
        set { SetValue(MapsProperty, value); }
    }

    private static void MapsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var list = (MapList)sender;
        list.SelectedMaps = new();
    }

    public MapList()
    {
        InitializeComponent();
    }

    private void Map_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var map = (Map)((FrameworkElement)sender).DataContext;
        if (SelectedMaps.Contains(map))
            SelectedMaps.Remove(map);
        else
            SelectedMaps.Add(map);
    }
}
