using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ImageMap4;

public class MainViewModel : ObservableObject
{
    public ObservableCollection<JavaWorld> JavaWorlds { get; } = new();
    public ObservableCollection<BedrockWorld> BedrockWorlds { get; } = new();
    
    private IWorld? _selectedWorld;
    public IWorld? SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            _selectedWorld = value;
            OnPropertyChanged();
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
}