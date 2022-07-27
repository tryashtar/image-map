using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;
public class MainViewModel
{
    public ObservableCollection<World> JavaWorlds { get; }
    public ObservableCollection<World> BedrockWorlds { get; }
    public World SelectedWorld { get; set; }

    public MainViewModel()
    {
        JavaWorlds = new();
        BedrockWorlds = new();
        RefreshWorlds();
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
                if (File.Exists(Path.Combine(dir, "level.dat")))
                    BedrockWorlds.Add(new BedrockWorld(dir));
            }
        }
    }
}
