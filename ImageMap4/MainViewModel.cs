﻿using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageMap4;
public class MainViewModel : ObservableObject
{
    public ICommand TransferAllCommand { get; }
    public ObservableCollection<World> JavaWorlds { get; }
    public ObservableCollection<World> BedrockWorlds { get; }
    public ObservableCollection<Selectable<Map>> ImportingMaps { get; private set; }
    public ObservableCollection<Selectable<Map>> ExistingMaps { get; private set; }
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

    public bool ShowEmptyMaps
    {
        get { return Properties.Settings.Default.ShowEmptyMaps; }
        set { Properties.Settings.Default.ShowEmptyMaps = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        JavaWorlds = new();
        BedrockWorlds = new();
        ImportingMaps = new();
        ExistingMaps = new();
        TransferAllCommand = new RelayCommand(() =>
        {
            SelectedWorld.AddMaps(ImportingMaps.Select(x => x.Item));
            foreach (var item in ImportingMaps)
            {
                ExistingMaps.Add(new Selectable<Map>(item.Item));
            }
            ImportingMaps.Clear();
        });
        RefreshWorlds();
    }

    public void RefreshMaps()
    {
        ImportingMaps = new();
        ExistingMaps = new(SelectedWorld.GetMaps().OrderBy(x => x.ID).Select(x => new Selectable<Map>(x)));
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

    public void AddImport(ImportSettings settings)
    {
        long id = ImportingMaps.Concat(ExistingMaps).Select(x => x.Item.ID).DefaultIfEmpty(-1).Max() + 1;
        foreach (var item in SelectedWorld.MakeMaps(settings))
        {
            ImportingMaps.Add(new Selectable<Map>(new Map(id, item)));
            id++;
        }
    }
}
