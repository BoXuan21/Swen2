using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class DeleteTourPopupViewModel : INotifyPropertyChanged
    {
        private string _tourName;
        public string TourName
        {
            get => _tourName;
            set
            {
                _tourName = value;
                OnPropertyChanged(nameof(TourName));
            }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? RequestClose;
        public event Action<bool>? DeleteConfirmed; // true if confirmed, false if cancelled

        public DeleteTourPopupViewModel(string tourName)
        {
            TourName = tourName;
            ConfirmCommand = new RelayCommand(OnConfirm);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnConfirm()
        {
            DeleteConfirmed?.Invoke(true);
            RequestClose?.Invoke();
        }

        private void OnCancel()
        {
            DeleteConfirmed?.Invoke(false);
            RequestClose?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 