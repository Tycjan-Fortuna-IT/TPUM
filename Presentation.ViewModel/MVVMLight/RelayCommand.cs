using System;
using System.Windows.Input; // Core ICommand interface (usually from System.ObjectModel or System.Windows.Input contract)

namespace Presentation.ViewModel
{
    // A command whose sole purpose is to relay its functionality
    // to other objects by invoking delegates.
    // This version is UI-framework-agnostic.
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        // The event required by ICommand
        public event EventHandler? CanExecuteChanged;

        // Constructor for commands that can always execute.
        public RelayCommand(Action<object?> execute)
            : this(execute, null)
        {
        }

        // Constructor for commands with execution logic and conditional logic.
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Method used by the ViewModel to manually trigger a re-evaluation of CanExecute.
        public void RaiseCanExecuteChanged()
        {
            // Invoke the event handler if it's not null
            // This will notify the UI (e.g., Button) to re-query CanExecute.
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // Determines whether the command can execute in its current state.
        // Called by the UI element (e.g., Button.IsEnabled).
        public bool CanExecute(object? parameter)
        {
            // If _canExecute is null, the command is always enabled.
            // Otherwise, call the predicate passed into the constructor.
            return _canExecute == null || _canExecute(parameter);
        }

        // Defines the method to be called when the command is invoked.
        // Called by the UI element when it's triggered (e.g., Button Click).
        public void Execute(object? parameter)
        {
            // Call the action delegate passed into the constructor.
            _execute(parameter);
        }
    }
}