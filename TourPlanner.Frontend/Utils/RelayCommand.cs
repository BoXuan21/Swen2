using System;
using System.Windows.Input;

namespace TourPlanner.Frontend.Utils
{
    /// <summary>
    /// A command implementation that can always execute.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action? _execute;
        private readonly Func<bool>? _canExecute;
        private readonly Action<object?>? _executeWithParam;
        private readonly Func<object?, bool>? _canExecuteWithParam;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteWithParam = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecuteWithParam != null)
                return _canExecuteWithParam(parameter);

            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            if (_executeWithParam != null)
            {
                _executeWithParam(parameter);
            }
            else
            {
                _execute?.Invoke();
            }
        }

        /// <summary>
        /// Method used to raise the CanExecuteChanged event to indicate that the return value of the CanExecute method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
}
