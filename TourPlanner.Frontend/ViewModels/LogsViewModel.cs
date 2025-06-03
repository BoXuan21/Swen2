using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;
using System.Diagnostics;
using TourPlanner.Frontend.Models;
using System.Text.Json;
using System.Windows;
using TourPlanner.Frontend.Services;

namespace TourPlanner.Frontend.ViewModels
{
    public class LogsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourLog> _logs;
        private ObservableCollection<TourViewModel> _availableTours;
        private TourViewModel? _selectedTour;
        private TourLog? _selectedLog;
        private readonly TourApiClient _apiClient;

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

        public TourLog? SelectedLog
        {
            get => _selectedLog;
            set
            {
                if (_selectedLog != value)
                {
                    _selectedLog = value;
                    OnPropertyChanged(nameof(SelectedLog));
                    Debug.WriteLine($"SelectedLog changed to: {_selectedLog?.Id ?? "null"}");
                    ((RelayCommand)EditLogCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteLogCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AddLogCommand { get; }
        public ICommand EditLogCommand { get; }
        public ICommand DeleteLogCommand { get; }
        public ICommand LoadLogsCommand { get; }
        public ICommand LoadToursCommand { get; }

        public event Action? RequestOpenAddLogPopup;
        public event Action<TourLog>? RequestOpenEditLogPopup;
        public event Action<TourLog>? RequestOpenDeleteLogPopup;

        public LogsViewModel()
        {
            _logs = new ObservableCollection<TourLog>();
            _availableTours = new ObservableCollection<TourViewModel>();
            AddLogCommand = new RelayCommand(OnAddLog, CanAddLog);
            EditLogCommand = new RelayCommand(OnEditLog, CanEditLog);
            DeleteLogCommand = new RelayCommand(OnDeleteLog, CanDeleteLog);
            LoadLogsCommand = new RelayCommand(
                async (parameter) =>
                {
                    if (parameter is string tourId)
                    {
                        await LoadLogsAsync(tourId);
                    }
                },
                (parameter) => parameter is string && !string.IsNullOrEmpty((string)parameter)
            );
            LoadToursCommand = new RelayCommand(async () => await LoadToursAsync(), () => true);
            _apiClient = new TourApiClient();
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

        private async Task LoadToursAsync()
        {
            try
            {
                Debug.WriteLine("Loading tours for logs tab");
                var tours = await _apiClient.GetToursAsync();
                AvailableTours.Clear();

                foreach (var tour in tours)
                {
                    AvailableTours.Add(new TourViewModel
                    {
                        Id = tour["id"]?.ToString() ?? string.Empty,
                        Name = tour["name"]?.ToString() ?? string.Empty
                    });
                }

                Debug.WriteLine($"Loaded {AvailableTours.Count} tours");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tours: {ex.Message}");
            }
        }

        private async Task LoadLogsAsync(string tourId)
        {
            try
            {
                Debug.WriteLine($"Loading logs for tour {tourId}");
                var logs = await _apiClient.GetLogsByTourIdAsync(tourId);
                Debug.WriteLine($"Received {logs?.Count ?? 0} logs from API");

                Logs.Clear();

                if (logs != null)
                {
                    foreach (var log in logs)
                    {
                        TimeSpan duration;

                        // Try different approaches to parse the duration
                        if (log["duration"] != null)
                        {
                            if (log["duration"].GetValueKind() == JsonValueKind.Number)
                            {
                                // If it's a number, assume it's minutes
                                double minutes = log["duration"].GetValue<double>();
                                duration = TimeSpan.FromMinutes(minutes);
                                Debug.WriteLine($"Parsed duration as minutes: {minutes} -> {duration}");
                            }
                            else if (log["duration"].GetValueKind() == JsonValueKind.String)
                            {
                                // If it's a string, try to parse as TimeSpan
                                string durationStr = log["duration"].GetValue<string>();
                                if (TimeSpan.TryParse(durationStr, out var parsedDuration))
                                {
                                    duration = parsedDuration;
                                    Debug.WriteLine($"Parsed duration from string: {durationStr} -> {duration}");
                                }
                                else
                                {
                                    // Default to zero if parsing fails
                                    duration = TimeSpan.Zero;
                                    Debug.WriteLine($"Failed to parse duration string: {durationStr}, using zero");
                                }
                            }
                            else
                            {
                                duration = TimeSpan.Zero;
                                Debug.WriteLine($"Unknown duration format, using zero");
                            }
                        }
                        else
                        {
                            duration = TimeSpan.Zero;
                            Debug.WriteLine("Duration field is null, using zero");
                        }

                        var tourLog = new TourLog
                        {
                            Id = log["id"]?.ToString() ?? string.Empty,
                            TourId = log["tourId"]?.ToString() ?? string.Empty,
                            Date = log["date"]?.GetValue<DateTime>() ?? DateTime.Now,
                            Comment = log["comment"]?.ToString() ?? string.Empty,
                            Difficulty = log["difficulty"]?.GetValue<int>() ?? 1,
                            Rating = log["rating"]?.GetValue<int>() ?? 1,
                            Duration = duration
                        };

                        Logs.Add(tourLog);
                        Debug.WriteLine($"Added log: {tourLog.Id} - {tourLog.Comment} - Duration: {tourLog.Duration}");
                    }

                    Debug.WriteLine($"Total logs loaded: {Logs.Count}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading logs: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
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