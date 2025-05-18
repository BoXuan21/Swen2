using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class ModifyTourPopupViewModel : INotifyPropertyChanged
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

        private string _from;
        public string From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
            }
        }

        private string _to;
        public string To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
            }
        }

        private string _distance;
        public string Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        private string _transportType = "Car";
        public string TransportType
        {
            get => _transportType;
            set
            {
                _transportType = value;
                OnPropertyChanged(nameof(TransportType));
            }
        }

        public ICommand ModifyCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? RequestClose;
        public event Action<bool>? ModificationConfirmed;

        public ModifyTourPopupViewModel(string tourName, string from, string to, string distance, string transportType)
        {
            // Initialize with existing tour data
            TourName = tourName;
            From = from;
            To = to;
            Distance = distance;
            TransportType = transportType;

            ModifyCommand = new RelayCommand(OnModify);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnModify()
        {
            ModificationConfirmed?.Invoke(true);
            RequestClose?.Invoke();
        }

        private void OnCancel()
        {
            ModificationConfirmed?.Invoke(false);
            RequestClose?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 