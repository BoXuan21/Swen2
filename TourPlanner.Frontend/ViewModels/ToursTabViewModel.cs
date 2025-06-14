using System;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace TourPlanner.Frontend.ViewModels
{
    public class ToursTabViewModel : INotifyPropertyChanged
    {
        public ICommand OpenCreatePopupCommand { get; }
        public ICommand OpenDeletePopupCommand { get; }
        public ICommand OpenModifyPopupCommand { get; }

        public event Action? RequestOpenCreatePopup;
        public event Action<string>? RequestOpenDeletePopup;
        public event Action<string>? RequestOpenModifyPopup;

        private string? _selectedTourName;
        public string? SelectedTourName
        {
            get => _selectedTourName;
            set
            {
                _selectedTourName = value;
                OnPropertyChanged(nameof(SelectedTourName));
                ((RelayCommand)OpenDeletePopupCommand).RaiseCanExecuteChanged();
                ((RelayCommand)OpenModifyPopupCommand).RaiseCanExecuteChanged();
            }
        }

        private string? _selectedTourDescription;
        public string? SelectedTourDescription
        {
            get => _selectedTourDescription;
            set { _selectedTourDescription = value; OnPropertyChanged(nameof(SelectedTourDescription)); }
        }

        private string? _selectedTourFrom;
        public string? SelectedTourFrom
        {
            get => _selectedTourFrom;
            set { _selectedTourFrom = value; OnPropertyChanged(nameof(SelectedTourFrom)); }
        }

        private string? _selectedTourTo;
        public string? SelectedTourTo
        {
            get => _selectedTourTo;
            set { _selectedTourTo = value; OnPropertyChanged(nameof(SelectedTourTo)); }
        }

        private string? _selectedTourTransportType;
        public string? SelectedTourTransportType
        {
            get => _selectedTourTransportType;
            set { _selectedTourTransportType = value; OnPropertyChanged(nameof(SelectedTourTransportType)); }
        }

        private string? _selectedTourDistance;
        public string? SelectedTourDistance
        {
            get => _selectedTourDistance;
            set { _selectedTourDistance = value; OnPropertyChanged(nameof(SelectedTourDistance)); }
        }

        private string? _selectedTourEstimatedTime;
        public string? SelectedTourEstimatedTime
        {
            get => _selectedTourEstimatedTime;
            set { _selectedTourEstimatedTime = value; OnPropertyChanged(nameof(SelectedTourEstimatedTime)); }
        }

        public ObservableCollection<string> TourNames { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ToursTabViewModel()
        {
            OpenCreatePopupCommand = new RelayCommand(OnOpenCreatePopup);
            OpenDeletePopupCommand = new RelayCommand(OnOpenDeletePopup, CanOpenDeletePopup);
            OpenModifyPopupCommand = new RelayCommand(OnOpenModifyPopup, CanOpenModifyPopup);
        }

        private void OnOpenCreatePopup()
        {
            RequestOpenCreatePopup?.Invoke();
        }

        private void OnOpenDeletePopup()
        {
            if (SelectedTourName != null)
            {
                RequestOpenDeletePopup?.Invoke(SelectedTourName);
            }
        }

        private void OnOpenModifyPopup()
        {
            if (SelectedTourName != null)
            {
                RequestOpenModifyPopup?.Invoke(SelectedTourName);
            }
        }

        private bool CanOpenDeletePopup()
        {
            return !string.IsNullOrEmpty(SelectedTourName);
        }

        private bool CanOpenModifyPopup()
        {
            return !string.IsNullOrEmpty(SelectedTourName);
        }
    }
}
