using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;
public class MainViewModel : ObservableObject
{
    public ObservableCollection<World> JavaWorlds { get; }
    public ObservableCollection<World> BedrockWorlds { get; }
    public ObservableCollection<Map> ImportingMaps { get; private set; }
    public ObservableCollection<Map> ExistingMaps { get; private set; }
    private World _selectedWorld;
    public World SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            _selectedWorld = value;
            OnPropertyChanged();
            RefreshMaps();
        }
    }

    public MainViewModel()
    {
        JavaWorlds = new();
        BedrockWorlds = new();
        ImportingMaps = new();
        ExistingMaps = new();
        RefreshWorlds();
    }

    public void RefreshMaps()
    {
        ImportingMaps = new();
        ExistingMaps = new(SelectedWorld.GetMaps().OrderBy(x => x.ID));
        OnPropertyChanged(nameof(ImportingMaps));
        OnPropertyChanged(nameof(ExistingMaps));
    }

    public void RefreshWorlds()
    {
        JavaWorlds.Clear();
        BedrockWorlds.Clear();
        var java_dir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolder);
        if (Directory.Exists(java_dir))
        {
            foreach (var dir in Directory.GetDirectories(java_dir))
            {
                if (File.Exists(Path.Combine(dir, "level.dat")))
                    JavaWorlds.Add(new JavaWorld(dir));
            }
        }
        var bedrock_dir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolder);
        if (Directory.Exists(bedrock_dir))
        {
            foreach (var dir in Directory.GetDirectories(bedrock_dir))
            {
                if (File.Exists(Path.Combine(dir, "level.dat")) && Directory.Exists(Path.Combine(dir, "db")))
                    BedrockWorlds.Add(new BedrockWorld(dir));
            }
        }
    }
}
