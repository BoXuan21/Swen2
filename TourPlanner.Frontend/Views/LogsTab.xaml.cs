using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using TourPlanner.Frontend.Popups;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using TourPlanner.Frontend.Models;
using System.Text.Json;

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        private readonly LogsViewModel _viewModel;
        private readonly TourApiClient _apiClient;

        public LogsTab()
        {
            InitializeComponent();
            _apiClient = new TourApiClient();
            _viewModel = new LogsViewModel();
            DataContext = _viewModel;

            _viewModel.RequestOpenAddLogPopup += OnRequestOpenAddLogPopup;
            _viewModel.RequestOpenEditLogPopup += OnRequestOpenEditLogPopup;
            _viewModel.RequestOpenDeleteLogPopup += OnRequestOpenDeleteLogPopup;

            Loaded += LogsTab_Loaded;
        }

        private async void LogsTab_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await LoadToursAsync();
        }

        private async Task LoadToursAsync()
        {
            try
            {
                Debug.WriteLine("Loading tours for logs tab");
                var tours = await _apiClient.GetToursAsync();
                _viewModel.AvailableTours.Clear();

                foreach (var tour in tours)
                {
                    _viewModel.AvailableTours.Add(new TourViewModel
                    {
                        Id = tour["id"]?.ToString() ?? string.Empty,
                        Name = tour["name"]?.ToString() ?? string.Empty
                    });
                }

                Debug.WriteLine($"Loaded {_viewModel.AvailableTours.Count} tours");
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
                
                _viewModel.Logs.Clear();
                
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
                        
                        _viewModel.Logs.Add(tourLog);
                        Debug.WriteLine($"Added log: {tourLog.Id} - {tourLog.Comment} - Duration: {tourLog.Duration}");
                    }
                    
                    Debug.WriteLine($"Total logs loaded: {_viewModel.Logs.Count}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading logs: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnRequestOpenAddLogPopup()
        {
            var popup = new Popups.AddLogPopup(_viewModel.SelectedTour!);
            if (popup.ShowDialog() == true)
            {
                await LoadLogsAsync(_viewModel.SelectedTour!.Id);
            }
        }

        private async void OnRequestOpenEditLogPopup(TourLog log)
        {
            var popup = new Popups.EditLogPopup(log);
            if (popup.ShowDialog() == true)
            {
                await LoadLogsAsync(log.TourId);
            }
        }

        private async void OnRequestOpenDeleteLogPopup(TourLog log)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this log?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiClient.DeleteLogAsync(log.Id);
                    await LoadLogsAsync(log.TourId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting log: {ex.Message}");
                    MessageBox.Show(
                        $"Error deleting log: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private async void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("TourComboBox_SelectionChanged called");
            if (_viewModel.SelectedTour != null)
            {
                Debug.WriteLine($"Tour selected: {_viewModel.SelectedTour.Name} (ID: {_viewModel.SelectedTour.Id})");
                await LoadLogsAsync(_viewModel.SelectedTour.Id);
            }
            else
            {
                Debug.WriteLine("No tour selected");
                _viewModel.Logs.Clear();
            }
        }
    }
}
