﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.Frontend.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        private readonly Action<object?> _executeWithParam;
        private readonly Func<object?, bool>? _canExecuteWithParam;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _executeWithParam = execute;
            _canExecuteWithParam = canExecute;
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

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
