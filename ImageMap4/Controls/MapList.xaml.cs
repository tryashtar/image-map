using System;
using System.Collections;
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
    public static readonly DependencyProperty MapMenuProperty =
            DependencyProperty.Register(nameof(MapMenu), typeof(ContextMenu),
            typeof(MapList), new FrameworkPropertyMetadata());
    public ContextMenu MapMenu
    {
        get { return (ContextMenu)GetValue(MapMenuProperty); }
        set { SetValue(MapMenuProperty, value); }
    }
    // icon that appears next to the map name (used for warnings)
    public DataTemplate Status { get; set; } = new();
    // can't be much more specific than IEnumerable because we bind an ICollectionView to this
    public IEnumerable<Selectable<Map>> Maps => ((IEnumerable)DataContext).Cast<Selectable<Map>>();
    public ICommand SelectAllCommand { get; }
    public ICommand DeselectAllCommand { get; }

    public MapList()
    {
        InitializeComponent();
        SelectAllCommand = new RelayCommand(() =>
        {
            foreach (var item in Maps)
            {
                item.IsSelected = true;
            }
        });
        DeselectAllCommand = new RelayCommand(() =>
        {
            foreach (var item in Maps)
            {
                item.IsSelected = false;
            }
        });
    }

    private Selectable<Map>? LastClicked;
    private void Map_MouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Focus();
        var map = (Selectable<Map>)((FrameworkElement)sender).DataContext;
        if (e.RightButton == MouseButtonState.Pressed)
        {
            if (!map.IsSelected)
            {
                foreach (var item in Maps)
                {
                    item.IsSelected = false;
                }
                map.IsSelected = true;
            }
        }
        else
        {
            map.IsSelected = !map.IsSelected;
            // yes this is bad... maybe we should enumerate the controls instead?
            var maps = Maps.ToList();
            if (LastClicked != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                int first = maps.IndexOf(LastClicked);
                int last = maps.IndexOf(map);
                for (int i = Math.Min(first, last); i <= Math.Max(first, last); i++)
                {
                    maps[i].IsSelected = map.IsSelected;
                }
            }
        }
        LastClicked = map;
    }

    private void List_MouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Focus();
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
        if (Item is INotifyPropertyChanged p)
            p.PropertyChanged += Item_PropertyChanged;
    }

    // hack to make sure changes to maps' IDs bubble up to ObservableList
    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }
}
