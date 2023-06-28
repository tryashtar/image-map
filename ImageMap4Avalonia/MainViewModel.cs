using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using TryashtarUtils.Utility;

namespace ImageMap4;

public class MainViewModel : ObservableObject
{
    public ObservableCollection<JavaWorld> JavaWorlds { get; } = new();
    public ObservableCollection<BedrockWorld> BedrockWorlds { get; } = new();
    public ObservableList<Selectable<Map>> ImportingMaps { get; } = new();
    public ObservableList<Selectable<Map>> ExistingMaps { get; } = new();
    public ReadOnlyCollection<IInventory>? PlayerList { get; private set; }
    
    private readonly UndoHistory UndoHistory = new();
    private CancellationTokenSource MapCTS = new();
    private IWorld? _selectedWorld;
    public IWorld? SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            _selectedWorld = value;
            UndoHistory.Clear();
            OnPropertyChanged();
            MapCTS?.Cancel();
            MapCTS?.Dispose();
            if (_selectedWorld != null)
            {
                MapCTS = new();
                _ = RefreshMaps(MapCTS.Token);
                var inventories = _selectedWorld.GetInventories();
                if (_selectedWorld is JavaWorld)
                    inventories = inventories.Select(x => new DisplayJavaInventory(x));
                var players = inventories.ToList();
                players.Insert(0, new NoInventory());
                PlayerList = players.AsReadOnly();
                OnPropertyChanged(nameof(PlayerList));
            }
        }
    }

    public MainViewModel()
    {
        RefreshWorlds();
    }

    private static IEnumerable<JavaWorld> GetJavaWorlds(string directory)
    {
        foreach (var dir in Directory.GetDirectories(directory))
        {
            if (!File.Exists(Path.Combine(dir, "level.dat"))) continue;
            JavaWorld world;
            try
            {
                world = new JavaWorld(dir);
            }
            catch
            {
                continue;
            }

            yield return world;
        }
    }

    private static IEnumerable<BedrockWorld> GetBedrockWorlds(string directory)
    {
        foreach (var dir in Directory.GetDirectories(directory))
        {
            if (!File.Exists(Path.Combine(dir, "level.dat")) ||
                !Directory.Exists(Path.Combine(dir, "db"))) continue;
            BedrockWorld world;
            try
            {
                world = new BedrockWorld(dir);
            }
            catch
            {
                continue;
            }

            yield return world;
        }
    }

    public void RefreshWorlds()
    {
        JavaWorlds.Clear();
        BedrockWorlds.Clear();
        Properties.Settings.Default.JavaFolders ??= new();
        foreach (var raw_dir in Properties.Settings.Default.JavaFolders)
        {
            string java_dir = Environment.ExpandEnvironmentVariables(raw_dir);
            if (!Directory.Exists(java_dir)) continue;

            foreach (var world in GetJavaWorlds(java_dir).OrderByDescending(x => x.AccessDate))
            {
                JavaWorlds.Add(world);
            }
        }

        Properties.Settings.Default.BedrockFolders ??= new();
        foreach (var raw_dir in Properties.Settings.Default.BedrockFolders)
        {
            string bedrock_dir = Environment.ExpandEnvironmentVariables(raw_dir);
            if (!Directory.Exists(bedrock_dir)) continue;

            foreach (var world in GetBedrockWorlds(bedrock_dir).OrderByDescending(x => x.AccessDate))
            {
                BedrockWorlds.Add(world);
            }
        }
    }
    
    private static readonly IComparer<Selectable<Map>> Sorter = new LambdaComparer<Selectable<Map>, long>(x => x.Item.ID);
    public async Task RefreshMaps(CancellationToken ct)
    {
        ImportingMaps.Clear();
        ExistingMaps.Clear();
        if (SelectedWorld == null)
            return;
        await Parallel.ForEachAsync(SelectedWorld.GetMapsAsync(), new ParallelOptions() { CancellationToken = ct, MaxDegreeOfParallelism = 5 }, async (item, token) =>
        {
            token.ThrowIfCancellationRequested();
            var selectable = new Selectable<Map>(item);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (!token.IsCancellationRequested)
                    Insert(selectable, ExistingMaps, Sorter);
            });
        });
    }
    
    private void Insert<T>(T item, IList<T> list, IComparer<T> sorter)
    {
        int index = ListUtils.BinarySearch<T>(list, item, sorter);
        if (index < 0)
            index = ~index;
        list.Insert(index, item);
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
