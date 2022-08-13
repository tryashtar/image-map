using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for MapList.xaml
/// </summary>
public partial class MapList : UserControl
{
    public IList<Selectable<Map>> Maps => (IList<Selectable<Map>>)DataContext;
    private Selectable<Map>? LastClicked;

    public MapList()
    {
        InitializeComponent();
    }

    private void Map_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var map = (Selectable<Map>)((FrameworkElement)sender).DataContext;
        map.IsSelected = !map.IsSelected;
        if (LastClicked != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
        {
            int first = Maps.IndexOf(LastClicked);
            int last = Maps.IndexOf(map);
            for (int i = Math.Min(first, last); i <= Math.Max(first, last); i++)
            {
                Maps[i].IsSelected = map.IsSelected;
            }
        }
        LastClicked = map;
    }
}

public class Selectable<T> : ObservableObject
{
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
    }

    public T Item { get; }
    public Selectable(T item, bool selected = false)
    {
        Item = item;
        _isSelected = selected;
    }
}
