﻿using System.Windows.Input;

namespace SecureApp.ViewModels;

public class ViewModelCommand : ICommand
{
    private readonly Action<object> _executeAction;
    private readonly Predicate<object>? _canExecuteAction;

    public ViewModelCommand(Action<object> executeAction)
    {
        _executeAction = executeAction;
        _canExecuteAction = null;
    }

    public ViewModelCommand(Action<object> executeAction, Predicate<object> canExecuteAction)
    {
        _executeAction = executeAction;
        _canExecuteAction = canExecuteAction;
    }
    
    public bool CanExecute(object? parameter)
    {
        return _canExecuteAction is null ? true : _canExecuteAction(parameter);
    }

    public void Execute(object? parameter)
    {
        _executeAction(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}