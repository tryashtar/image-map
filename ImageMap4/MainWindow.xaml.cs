using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMap4;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)this.DataContext;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void JavaFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.JavaFolder);
        if (dialog.ShowDialog() == true)
        {
            Properties.Settings.Default.JavaFolder = dialog.SelectedPath;
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private void BedrockFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.SelectedPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.BedrockFolder);
        if (dialog.ShowDialog() == true)
        {
            Properties.Settings.Default.BedrockFolder = dialog.SelectedPath;
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).RefreshWorlds();
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Multiselect = true;
        dialog.Title = "Select images to import";
        dialog.Filter = "Image Files|*.png; *.bmp, *.jpg, *.jpeg, *.gif|All Files|*";
        if (dialog.ShowDialog() == true)
            OpenImages(dialog.FileNames);
    }

    private void OpenImages(IEnumerable<string> images)
    {
        var import = new ImportWindow(images);
        import.ShowDialog();
        ViewModel.AddImports(import.Maps);
    }
}
