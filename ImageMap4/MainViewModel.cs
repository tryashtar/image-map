using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public ICommand DiscardCommand { get; }
    public ICommand DeleteCommand { get; }
    public ObservableCollection<JavaWorld> JavaWorlds { get; } = new();
    public ObservableCollection<BedrockWorld> BedrockWorlds { get; } = new();
    public ObservableList<Selectable<Map>> ImportingMaps { get; } = new();
    public ObservableList<Selectable<Map>> ExistingMaps { get; } = new();
    public List<StructureGrid> ImportingStructures { get; } = new();
    public ReadOnlyCollection<IInventory>? PlayerList { get; private set; }
    public ICollectionView ExistingMapsView
    {
        get { return CollectionViewSource.GetDefaultView(ExistingMaps); }
    }
    private readonly UndoHistory UndoHistory = new();
    private World? _selectedWorld;
    public World? SelectedWorld
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
        ImportingMaps.CollectionChanged += Maps_CollectionChanged;
        ExistingMaps.CollectionChanged += Maps_CollectionChanged;
        TransferAllCommand = new RelayCommand(() =>
        {
            if (SelectedWorld == null)
                return;
            var overwritten = ExistingMaps.Where(x => ConflictingIDs.Contains(x.Item.ID)).ToList();
            var importing = ImportingMaps.ToList();
            int index = Properties.Settings.Default.InventoryChoice;
            if (index < 0 || index >= PlayerList.Count)
                index = 1;
            var inventory = PlayerList[index];
            if (CreateStructures)
                SelectedWorld.AddStructures(ImportingStructures, inventory);
            ImportingStructures.Clear();
            UndoHistory.Perform(() =>
            {
                SelectedWorld.AddMaps(importing.Select(x => x.Item));
                InsertRange(importing, ExistingMaps, Sorter);
                RemoveRange(overwritten, ExistingMaps);
                ImportingMaps.Clear();
            }, () =>
            {
                SelectedWorld.RemoveMaps(importing.Select(x => x.Item.ID));
                SelectedWorld.AddMaps(overwritten.Select(x => x.Item));
                RemoveRange(importing, ExistingMaps);
                InsertRange(overwritten, ExistingMaps, Sorter);
                InsertRange(importing, ImportingMaps, Sorter);
            });
        });
        UndoCommand = new RelayCommand(() => UndoHistory.Undo());
        RedoCommand = new RelayCommand(() => UndoHistory.Redo());
        DiscardCommand = new RelayCommand<IList<Selectable<Map>>>(x =>
        {
            var maps = x.Where(x => x.IsSelected).ToList();
            UndoHistory.Perform(() =>
            {
                RemoveRange(maps, x);
            }, () =>
            {
                InsertRange(maps, x, Sorter);
            });
        });
        DeleteCommand = new RelayCommand<IList<Selectable<Map>>>(x =>
        {
            var maps = x.Where(x => x.IsSelected).ToList();
            UndoHistory.Perform(() =>
            {
                RemoveRange(maps, x);
                SelectedWorld?.RemoveMaps(maps.Select(x => x.Item.ID));
            }, () =>
            {
                InsertRange(maps, x, Sorter);
                SelectedWorld?.AddMaps(maps.Select(x => x.Item));
            });
        });
        RefreshWorlds();
    }

    private void Maps_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateConflictingIDs();
    }

    private void Maps_ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Map.ID))
            UpdateConflictingIDs();
    }

    private void UpdateConflictingIDs()
    {
        ConflictingIDs.Clear();
        var conflicts = ExistingMaps.Select(x => x.Item.ID).Intersect(ImportingMaps.Select(x => x.Item.ID));
        foreach (var item in conflicts)
        {
            ConflictingIDs.Add(item);
        }
        OnPropertyChanged(nameof(ConflictingIDs));
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!token.IsCancellationRequested)
                    Insert(selectable, ExistingMaps, Sorter);
            });
        });
    }

    public void ChangeIDs(IList<Selectable<Map>> source, IEnumerable<Selectable<Map>> maps, long new_id)
    {
        long id = new_id;
        var changing = maps.Select(x => (from: x.Item.ID, to: id++, map: x.Item)).ToList();
        var new_ids = changing.Select(x => x.to).ToHashSet();
        var replaced = source.Except(maps).Where(x => new_ids.Contains(x.Item.ID)).ToList();
        var affected = maps.ToList();
        UndoHistory.Perform(() =>
        {
            foreach (var (_, to, map) in changing)
            {
                map.ID = to;
            }
            RemoveRange(replaced, source);
            RemoveRange(affected, source);
            InsertRange(affected, source, Sorter);
            if (source == ExistingMaps && SelectedWorld != null)
            {
                SelectedWorld.RemoveMaps(changing.Select(x => x.from));
                SelectedWorld.AddMaps(changing.Select(x => x.map));
            }
        }, () =>
        {
            foreach (var (from, _, map) in changing)
            {
                map.ID = from;
            }
            InsertRange(replaced, source, Sorter);
            RemoveRange(affected, source);
            InsertRange(affected, source, Sorter);
            if (source == ExistingMaps && SelectedWorld != null)
            {
                SelectedWorld.RemoveMaps(new_ids);
                SelectedWorld.AddMaps(changing.Select(x => x.map));
                SelectedWorld.AddMaps(replaced.Select(x => x.Item));
            }
        });
    }

    public void AutoIDs(IList<Selectable<Map>> source, IEnumerable<Selectable<Map>> maps)
    {
        ChangeIDs(source, maps, NextFreeID());
    }

    private void Insert<T>(T item, IList<T> list, IComparer<T> sorter)
    {
        int index = ListUtils.BinarySearch<T>(list, item, sorter);
        if (index < 0)
            index = ~index;
        list.Insert(index, item);
    }

    private void InsertRange<T>(IEnumerable<T> items, IList<T> list, IComparer<T> sorter)
    {
        foreach (var item in items)
        {
            Insert(item, list, sorter);
        }
    }

    private void RemoveRange<T>(IEnumerable<T> items, IList<T> list) where T : class
    {
        foreach (var item in items)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == item)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
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
        }
        Properties.Settings.Default.BedrockFolders ??= new();
        foreach (var raw_dir in Properties.Settings.Default.BedrockFolders)
        {
            string bedrock_dir = Environment.ExpandEnvironmentVariables(raw_dir);
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
    }

    private long NextFreeID()
    {
        return ImportingMaps.Concat(ExistingMaps).Select(x => x.Item.ID).Append(-1).Max() + 1;
    }

    public async Task AddImports(IList<ImportSettings> settings)
    {
        if (SelectedWorld == null)
            return;
        long curent_id = NextFreeID();
        var settings_list = new List<(ImportSettings settings, long first_id)>();
        foreach (var item in settings)
        {
            settings_list.Add((item, curent_id));
            curent_id += item.Width * item.Height;
        }
        var action = (CancellationToken token) =>
        {
            var added_maps = new List<Selectable<Map>>();
            var added_structures = new List<StructureGrid>();
            Parallel.ForEach(settings_list.ToList(), (stuff, state) =>
            {
                if (token.IsCancellationRequested)
                    state.Break();
                else
                {
                    var (setting, id) = stuff;
                    var map_data = SelectedWorld.MakeMaps(setting);
                    var maps = ListUtils.Map2D(map_data, x => new Map(id++, x));
                    var import = ListUtils.Flatten(maps).Select(x => new Selectable<Map>(x)).ToList();
                    var structure = new StructureGrid("imagemap:" + setting.Preview.Source.Name, maps)
                    {
                        GlowingFrames = Properties.Settings.Default.GlowingFrames,
                        InvisibleFrames = Properties.Settings.Default.InvisibleFrames
                    };
                    lock (settings)
                    {
                        added_maps.AddRange(import);
                        added_structures.Add(structure);
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            if (!token.IsCancellationRequested)
                                InsertRange(import, ImportingMaps, Sorter);
                        });
                        ImportingStructures.Add(structure);
                        settings.Remove(setting);
                    }
                }
            });
            return (added_maps, added_structures);
        };
        UndoHistory.PerformContext(((List<Selectable<Map>> done_maps, List<StructureGrid> done_structures)? context) =>
        {
            var maps = new List<Selectable<Map>>();
            var structures = new List<StructureGrid>();
            if (context.HasValue)
            {
                maps.AddRange(context.Value.done_maps);
                structures.AddRange(context.Value.done_structures);
                InsertRange(maps, ImportingMaps, Sorter);
                ImportingStructures.AddRange(structures);
            }
            var source = new CancellationTokenSource();
            var task = Task.Run(() => action(source.Token));
            return (source, task, maps, structures);
        }, context =>
        {
            var (source, task, done_maps, done_structures) = context;
            source.Cancel();
            task.Wait();
            var (maps, structures) = task.Result;
            done_maps.AddRange(maps);
            done_structures.AddRange(structures);
            RemoveRange(done_maps, ImportingMaps);
            RemoveRange(done_structures, ImportingStructures);
            return (done_maps, done_structures);
        });
    }
}
