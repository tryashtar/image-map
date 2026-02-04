using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace ImageMap4;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        File.WriteAllText($"crash-{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.txt", e.Exception.ToString());
    }
}
