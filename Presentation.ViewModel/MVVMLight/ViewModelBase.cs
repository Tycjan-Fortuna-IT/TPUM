using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentation.ViewModel
{
    // Base class for all ViewModels implementing INotifyPropertyChanged
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Raises the PropertyChanged event
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Helper method to set a field and raise PropertyChanged only if the value has changed
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // Use EqualityComparer<T>.Default to handle value types, reference types, and nulls correctly
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false; // Value hasn't changed
            }

            field = value; // Set the new value
            OnPropertyChanged(propertyName); // Raise the event
            return true; // Value has changed
        }
    }
}