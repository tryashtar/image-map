using System;
using System.Windows.Input;

namespace ImageMap4;

public class RelayCommand : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }

    private readonly Action ExecuteAction;
    public RelayCommand(Action execute)
    {
        ExecuteAction = execute;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        ExecuteAction();
    }
}

public class RelayCommand<T> : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }

    private readonly Action<T> ExecuteAction;
    public RelayCommand(Action<T> execute)
    {
        ExecuteAction = execute;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T casted)
            ExecuteAction(casted);
        else throw new InvalidCastException();
    }
}
