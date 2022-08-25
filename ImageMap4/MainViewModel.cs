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
    private World? _selectedWorld;
    public World? SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            if (_selectedWorld is BedrockWorld w)
                w.CloseDB();
            _selectedWorld = value;
            UndoHistory.Clear();
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
        ImportingMaps.CollectionChanged += Maps_CollectionChanged;
        ExistingMaps.CollectionChanged += Maps_CollectionChanged;
        TransferAllCommand = new RelayCommand(() =>
        {
            if (SelectedWorld == null)
                return;
            var overwritten = ExistingMaps.Where(x => ConflictingIDs.Contains(x.Item.ID)).ToList();
            var importing = ImportingMaps.ToList();
            var structures = ImportingStructures.ToList();
            int index = Properties.Settings.Default.InventoryChoice;
            if (index < 0 || index >= PlayerList.Count)
                index = 1;
            var inventory = PlayerList[index];
            bool send_structures = CreateStructures;
            UndoHistory.Perform(() =>
            {
                SelectedWorld.AddMaps(importing.Select(x => x.Item));
                if (send_structures)
                {
                    foreach (var structure in structures)
                    {
                        SelectedWorld.AddStructure(structure, inventory);
                    }
                }
                foreach (var item in importing)
                {
                    Insert(item, ExistingMaps);
                }
                RemoveRange(overwritten, ExistingMaps);
                ImportingMaps.Clear();
                ImportingStructures.Clear();
            }, () =>
            {
                SelectedWorld.RemoveMaps(importing.Select(x => x.Item.ID));
                SelectedWorld.AddMaps(overwritten.Select(x => x.Item));
                RemoveRange(importing, ExistingMaps);
                foreach (var item in overwritten)
                {
                    Insert(item, ExistingMaps);
                }
                ImportingMaps.AddRange(importing);
                ImportingStructures.AddRange(structures);
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
                foreach (var map in maps)
                {
                    Insert(map, x);
                }
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
                foreach (var map in maps)
                {
                    Insert(map, x);
                }
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
                    Insert(selectable, ExistingMaps);
            });
        });
    }

    public void ChangeIDs(IList<Selectable<Map>> source, IEnumerable<Selectable<Map>> maps, long new_id)
    {
        long id = new_id;
        var changing = maps.Select(x => (from: x.Item.ID, to: id++, map: x.Item)).ToList();
        var new_ids = changing.Select(x => x.to).ToHashSet();
        var replaced = source.Except(maps).Where(x => new_ids.Contains(x.Item.ID)).ToList();
        UndoHistory.Perform(() =>
        {
            foreach (var (_, to, map) in changing)
            {
                map.ID = to;
            }
            RemoveRange(replaced, source);
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
            foreach (var item in replaced)
            {
                Insert(item, source);
            }
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

    private void Insert(Selectable<Map> item, IList<Selectable<Map>> list)
    {
        int index = list.BinarySearch(item, Sorter);
        if (index < 0)
            index = ~index;
        list.Insert(index, item);
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

    private long NextFreeID()
    {
        return ImportingMaps.Concat(ExistingMaps).Select(x => x.Item.ID).Append(-1).Max() + 1;
    }

    public async Task AddImports(IList<Lazy<ImportSettings>> settings)
    {
        if (SelectedWorld == null)
            return;
        long id = NextFreeID();
        var action = (CancellationToken token) =>
        {
            var added_maps = new List<Selectable<Map>>();
            var added_structures = new List<StructureGrid>();
            var processed = new HashSet<ImportSettings>();
            Parallel.ForEach(settings.ToList(), (setting, state) =>
            {
                if (token.IsCancellationRequested)
                    state.Break();
                else
                {
                    var map_data = SelectedWorld.MakeMaps(setting.Value);
                    var maps = Map2D(map_data, x => new Map(id++, x));
                    var import = Flatten(maps).Select(x => new Selectable<Map>(x)).ToList();
                    var structure = new StructureGrid("imagemap:" + MakeSafe(setting.Value.Preview.Source.Name), maps)
                    {
                        GlowingFrames = Properties.Settings.Default.GlowingFrames,
                        InvisibleFrames = Properties.Settings.Default.InvisibleFrames
                    };
                    lock (processed)
                    {
                        added_maps.AddRange(import);
                        added_structures.Add(structure);
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            if (!token.IsCancellationRequested)
                                ImportingMaps.AddRange(import);
                        });
                        ImportingStructures.Add(structure);
                        processed.Add(setting.Value);
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
                ImportingMaps.AddRange(maps);
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

    private static string MakeSafe(string input)
    {
        input = Path.GetFileNameWithoutExtension(input);
        input = input.ToLower();
        input = input.Replace(' ', '_');
        return input;
    }
    private static IEnumerable<T> Flatten<T>(T[,] stuff)
    {
        for (int y = 0; y < stuff.GetLength(1); y++)
        {
            for (int x = 0; x < stuff.GetLength(0); x++)
            {
                yield return stuff[x, y];
            }
        }
    }
    private static TTo[,] Map2D<TFrom, TTo>(TFrom[,] stuff, Func<TFrom, TTo> func)
    {
        var result = new TTo[stuff.GetLength(0), stuff.GetLength(1)];
        for (int y = 0; y < stuff.GetLength(1); y++)
        {
            for (int x = 0; x < stuff.GetLength(0); x++)
            {
                result[x, y] = func(stuff[x, y]);
            }
        }
        return result;
    }
}
