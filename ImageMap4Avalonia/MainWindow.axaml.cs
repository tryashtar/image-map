using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ImageMap4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void JavaWorldsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.JavaFolders ??= new();
        bool adding = false;
        var dialog = new OpenFolderDialog();
        dialog.Title = adding ? "Add a Java world folder to the list" : "Select the folder where your Java worlds are saved";
        if (Properties.Settings.Default.JavaFolders.Count > 0)
            dialog.Directory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolders[0]);
        var path = await dialog.ShowAsync(this);
        if (path != null)
        {
            if (!adding)
                Properties.Settings.Default.JavaFolders.Clear();
            Properties.Settings.Default.JavaFolders.Add(path);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private async void BedrockWorldsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.BedrockFolders ??= new();
        bool adding = false;
        var dialog = new OpenFolderDialog();
        dialog.Title = adding ? "Add a Bedrock world folder to the list" : "Select the folder where your Bedrock worlds are saved";
        if (Properties.Settings.Default.BedrockFolders.Count > 0)
            dialog.Directory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolders[0]);
        var path = await dialog.ShowAsync(this);
        if (path != null)
        {
            if (!adding)
                Properties.Settings.Default.BedrockFolders.Clear();
            Properties.Settings.Default.BedrockFolders.Add(path);
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }
}