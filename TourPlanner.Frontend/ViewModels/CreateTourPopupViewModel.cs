using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class CreateTourPopupViewModel : INotifyPropertyChanged
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

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? RequestClose;

        public CreateTourPopupViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnCreate()
        {
            // Add validation and call service here if needed
            RequestClose?.Invoke();
        }

        private void OnCancel() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
