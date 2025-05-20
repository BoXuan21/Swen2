using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;
using System.Diagnostics;
using TourPlanner.Frontend.Models;

namespace TourPlanner.Frontend.ViewModels
{
    public class LogsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourLog> _logs;
        private ObservableCollection<TourViewModel> _availableTours;
        private TourViewModel? _selectedTour;
        private TourLogViewModel? _selectedLog;

        public ObservableCollection<TourLog> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        public ObservableCollection<TourViewModel> AvailableTours
        {
            get => _availableTours;
            set
            {
                _availableTours = value;
                OnPropertyChanged(nameof(AvailableTours));
            }
        }

        public TourViewModel? SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
                ((RelayCommand)AddLogCommand).RaiseCanExecuteChanged();
            }
        }

        public TourLogViewModel? SelectedLog
        {
            get => _selectedLog;
            set
            {
                _selectedLog = value;
                OnPropertyChanged(nameof(SelectedLog));
                ((RelayCommand)EditLogCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteLogCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddLogCommand { get; }
        public ICommand EditLogCommand { get; }
        public ICommand DeleteLogCommand { get; }

        public event Action? RequestOpenAddLogPopup;
        public event Action<TourLogViewModel>? RequestOpenEditLogPopup;
        public event Action<TourLogViewModel>? RequestOpenDeleteLogPopup;

        public LogsViewModel()
        {
            Logs = new ObservableCollection<TourLog>();
            AvailableTours = new ObservableCollection<TourViewModel>();
            AddLogCommand = new RelayCommand(OnAddLog, CanAddLog);
            EditLogCommand = new RelayCommand(OnEditLog, CanEditLog);
            DeleteLogCommand = new RelayCommand(OnDeleteLog, CanDeleteLog);
        }

        private void OnAddLog()
        {
            if (SelectedTour == null)
            {
                Debug.WriteLine("Cannot add log: No tour selected");
                return;
            }
            Debug.WriteLine($"Opening add log popup for tour: {SelectedTour.Name} (ID: {SelectedTour.Id})");
            RequestOpenAddLogPopup?.Invoke();
        }

        private void OnEditLog()
        {
            if (SelectedLog == null)
            {
                Debug.WriteLine("Cannot edit log: No log selected");
                return;
            }
            Debug.WriteLine($"Opening edit log popup for log: {SelectedLog.Id}");
            RequestOpenEditLogPopup?.Invoke(SelectedLog);
        }

        private void OnDeleteLog()
        {
            if (SelectedLog == null)
            {
                Debug.WriteLine("Cannot delete log: No log selected");
                return;
            }
            Debug.WriteLine($"Opening delete log popup for log: {SelectedLog.Id}");
            RequestOpenDeleteLogPopup?.Invoke(SelectedLog);
        }

        private bool CanAddLog() => SelectedTour != null;
        private bool CanEditLog() => SelectedLog != null;
        private bool CanDeleteLog() => SelectedLog != null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TourLogViewModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private DateTime _date;
        private string _comment = string.Empty;
        private int _difficulty;
        private int _rating;
        private TimeSpan _duration;
        private string _tourId = string.Empty;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        public int Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string TourId
        {
            get => _tourId;
            set
            {
                _tourId = value;
                OnPropertyChanged(nameof(TourId));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TourViewModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _name = string.Empty;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 