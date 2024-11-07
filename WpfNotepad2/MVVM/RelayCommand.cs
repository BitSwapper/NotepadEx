using System.Windows.Input;

namespace NotepadEx.MVVM;

public class RelayCommand : ICommand
{
    readonly Action execute;
    readonly Func<bool> canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter) => canExecute?.Invoke() ?? true;
    public void Execute(object parameter) => execute();
}

public class RelayCommand<T> : ICommand
{
    readonly Action<T> execute;
    readonly Func<T, bool> canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter) => canExecute?.Invoke((T)parameter) ?? true;
    public void Execute(object parameter) => execute((T)parameter);
}
