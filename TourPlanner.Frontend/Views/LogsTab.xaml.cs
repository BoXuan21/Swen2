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
                _viewModel.Logs.Clear();

                foreach (var log in logs)
                {
                    _viewModel.Logs.Add(new TourLog
                    {
                        Id = log["id"]?.ToString() ?? string.Empty,
                        TourId = log["tourId"]?.ToString() ?? string.Empty,
                        Date = log["date"]?.GetValue<DateTime>() ?? DateTime.Now,
                        Comment = log["comment"]?.ToString() ?? string.Empty,
                        Difficulty = log["difficulty"]?.GetValue<int>() ?? 1,
                        Rating = log["rating"]?.GetValue<int>() ?? 1,
                        Duration = TimeSpan.FromMinutes(log["duration"]?.GetValue<double>() ?? 0)
                    });
                }

                Debug.WriteLine($"Loaded {_viewModel.Logs.Count} logs");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading logs: {ex.Message}");
            }
        }

        private void OnRequestOpenAddLogPopup()
        {
            var popup = new Popups.AddLogPopup(_viewModel.SelectedTour!);
            if (popup.ShowDialog() == true)
            {
                LoadLogsAsync(_viewModel.SelectedTour!.Id);
            }
        }

        private void OnRequestOpenEditLogPopup(TourLogViewModel log)
        {
            var popup = new Popups.EditLogPopup(log);
            if (popup.ShowDialog() == true)
            {
                LoadLogsAsync(log.TourId);
            }
        }

        private async void OnRequestOpenDeleteLogPopup(TourLogViewModel log)
        {
            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to delete this log?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _apiClient.DeleteLogAsync(log.Id);
                    await LoadLogsAsync(log.TourId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting log: {ex.Message}");
                    System.Windows.MessageBox.Show(
                        $"Error deleting log: {ex.Message}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedTour != null)
            {
                await LoadLogsAsync(_viewModel.SelectedTour.Id);
            }
        }
    }
}
