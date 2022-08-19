using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TryashtarUtils.Utility;

namespace ImageMap4;
public class MainViewModel : ObservableObject
{
    public ICommand TransferAllCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ObservableCollection<JavaWorld> JavaWorlds { get; } = new();
    public ObservableCollection<BedrockWorld> BedrockWorlds { get; } = new();
    public ObservableList<Selectable<Map>> ImportingMaps { get; } = new();
    public ObservableList<Selectable<Map>> ExistingMaps { get; } = new();
    public ReadOnlyCollection<IInventory>? PlayerList { get; private set; }
    public ICollectionView ExistingMapsView
    {
        get { return CollectionViewSource.GetDefaultView(ExistingMaps); }
    }
    private World? _selectedWorld;
    public World? SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            _selectedWorld = value;
            OnPropertyChanged();
            MapCTS?.Cancel();
            MapCTS?.Dispose();
            if (_selectedWorld != null)
            {
                MapCTS = new();
                _ = RefreshMaps(MapCTS.Token);
                var players = _selectedWorld.GetInventories().ToList();
                players.Insert(0, new NoInventory());
                PlayerList = players.AsReadOnly();
                OnPropertyChanged(nameof(PlayerList));
            }
        }
    }
    private CancellationTokenSource MapCTS = new();

    public HashSet<long> ConflictingIDs { get; } = new();

    public bool ShowEmptyMaps
    {
        get { return Properties.Settings.Default.ShowEmptyMaps; }
        set { Properties.Settings.Default.ShowEmptyMaps = value; OnPropertyChanged(); ExistingMapsView.Refresh(); }
    }

    public bool CreateStructures
    {
        get { return Properties.Settings.Default.CreateStructures; }
        set { Properties.Settings.Default.CreateStructures = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        ExistingMapsView.Filter = x => ShowEmptyMaps || !((Selectable<Map>)x).Item.Data.IsEmpty;
        ImportingMaps.ItemChanged += Maps_ItemChanged;
        ExistingMaps.ItemChanged += Maps_ItemChanged;
        TransferAllCommand = new RelayCommand(() =>
        {
            SelectedWorld?.AddMaps(ImportingMaps.Select(x => x.Item));
            foreach (var item in ImportingMaps)
            {
                ExistingMaps.Add(new Selectable<Map>(item.Item));
            }
            ImportingMaps.Clear();
        });
        UndoCommand = new RelayCommand(() => UndoHistory.Undo());
        RedoCommand = new RelayCommand(() => UndoHistory.Redo());
        RefreshWorlds();
    }

    private void Maps_ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Map.ID))
        {
            ConflictingIDs.Clear();
            var conflicts = ExistingMaps.Select(x => x.Item.ID).Intersect(ImportingMaps.Select(x => x.Item.ID));
            foreach (var item in conflicts)
            {
                ConflictingIDs.Add(item);
            }
            OnPropertyChanged(nameof(ConflictingIDs));
        }
    }

    private static readonly IComparer<Selectable<Map>> Sorter = new LambdaComparer<Selectable<Map>, long>(x => x.Item.ID);
    public async Task RefreshMaps(CancellationToken ct)
    {
        ImportingMaps.Clear();
        ExistingMaps.Clear();
        if (SelectedWorld == null)
            return;
        await foreach (var item in SelectedWorld.GetMapsAsync())
        {
            ct.ThrowIfCancellationRequested();
            var selectable = new Selectable<Map>(item);
            int index = ExistingMaps.BinarySearch(selectable, Sorter);
            if (index < 0)
                index = ~index;
            ExistingMaps.Insert(index, selectable);
        }
    }

    public void RefreshWorlds()
    {
        JavaWorlds.Clear();
        BedrockWorlds.Clear();
        var java_dir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolder);
        if (Directory.Exists(java_dir))
        {
            IEnumerable<JavaWorld> get_worlds()
            {
                foreach (var dir in Directory.GetDirectories(java_dir))
                {
                    if (File.Exists(Path.Combine(dir, "level.dat")))
                        yield return new JavaWorld(dir);
                }
            }
            foreach (var world in get_worlds().OrderByDescending(x => x.AccessDate))
            {
                JavaWorlds.Add(world);
            }
        }
        var bedrock_dir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolder);
        if (Directory.Exists(bedrock_dir))
        {
            IEnumerable<BedrockWorld> get_worlds()
            {
                foreach (var dir in Directory.GetDirectories(bedrock_dir))
                {
                    if (File.Exists(Path.Combine(dir, "level.dat")) && Directory.Exists(Path.Combine(dir, "db")))
                        yield return new BedrockWorld(dir);
                }
            }
            foreach (var world in get_worlds().OrderByDescending(x => x.AccessDate))
            {
                BedrockWorlds.Add(world);
            }
        }
    }

    public void AddImport(ImportSettings settings)
    {
        if (SelectedWorld == null)
            return;
        long id = ImportingMaps.Concat(ExistingMaps).Select(x => x.Item.ID).Append(-1).Max() + 1;
        var maps = SelectedWorld.MakeMaps(settings).Select(x => new Selectable<Map>(new Map(id++, x))).ToList();
        UndoHistory.Perform(() =>
        {
            ImportingMaps.AddRange(maps);
        }, () =>
        {
            foreach (var item in maps)
            {
                for (int i = ImportingMaps.Count - 1; i >= 0; i--)
                {
                    if (ImportingMaps[i] == item)
                    {
                        ImportingMaps.RemoveAt(i);
                        break;
                    }
                }
            }
        });
    }
}
