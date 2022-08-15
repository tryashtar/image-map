using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;

public sealed class ObservableList<T> : ObservableCollection<T> where T : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? ItemChanged;
    public ObservableList()
    {
        this.CollectionChanged += WhenCollectionChanged;
    }

    public ObservableList(IEnumerable<T> items) : this()
    {
        AddRange(items);
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            this.Add(item);
        }
    }

    private void WhenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (INotifyPropertyChanged item in e.NewItems)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }
        if (e.OldItems != null)
        {
            foreach (INotifyPropertyChanged item in e.OldItems)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ItemChanged?.Invoke(sender, e);
    }
}