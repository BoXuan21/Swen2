using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using TourPlanner.Frontend.ViewModels.Base;
using TourPlanner.Frontend.Services;
using TourPlanner.Frontend.Models;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class TourWithLogs
    {
        public Tour Tour { get; set; }
        public ObservableCollection<TourLog> Logs { get; set; }

        public TourWithLogs(Tour tour)
        {
            Tour = tour;
            Logs = new ObservableCollection<TourLog>();
        }
    }

    public class ListsViewModel : ViewModelBase
    {
        private readonly TourApiClient _apiClient;
        private readonly DispatcherTimer _refreshTimer;
        private ObservableCollection<TourWithLogs> _tourWithLogs;

        public ICommand ExportToReportCommand { get; }
        public ICommand PrintListCommand { get; }
        public ICommand RefreshCommand { get; }

        public ObservableCollection<TourWithLogs> TourWithLogs
        {
            get => _tourWithLogs;
            set
            {
                _tourWithLogs = value;
                OnPropertyChanged();
            }
        }

        public ListsViewModel()
        {
            _apiClient = new TourApiClient();
            TourWithLogs = new ObservableCollection<TourWithLogs>();

            ExportToReportCommand = new RelayCommand(ExportToReport);
            PrintListCommand = new RelayCommand(PrintList);
            RefreshCommand = new RelayCommand(LoadData);

            // Set up auto-refresh timer
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30) // Refresh every 30 seconds
            };
            _refreshTimer.Tick += (s, e) => LoadData();
            _refreshTimer.Start();

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var tourObjects = await _apiClient.GetToursAsync();
                var newTourWithLogs = new ObservableCollection<TourWithLogs>();

                foreach (var tourObj in tourObjects)
                {
                    var tour = new Tour
                    {
                        Id = tourObj["id"]?.ToString() ?? string.Empty,
                        Name = tourObj["name"]?.ToString() ?? string.Empty,
                        Description = tourObj["description"]?.ToString() ?? string.Empty,
                        From = tourObj["fromLocation"]?.ToString() ?? string.Empty,
                        To = tourObj["toLocation"]?.ToString() ?? string.Empty,
                        TransportType = tourObj["transportType"]?.ToString() ?? string.Empty,
                        Distance = tourObj["distance"]?.GetValue<double>() ?? 0,
                        EstimatedTime = TimeSpan.FromMinutes(tourObj["estimatedTime"]?.GetValue<double>() ?? 0),
                        RouteImagePath = tourObj["routeInformation"]?.ToString() ?? string.Empty
                    };

                    var tourWithLogs = new TourWithLogs(tour);

                    // Load logs for this tour
                    var tourLogs = await _apiClient.GetLogsByTourIdAsync(tour.Id);
                    foreach (var logObj in tourLogs)
                    {
                        tourWithLogs.Logs.Add(new TourLog
                        {
                            Id = logObj["id"]?.ToString() ?? string.Empty,
                            TourId = logObj["tourId"]?.ToString() ?? string.Empty,
                            Date = DateTime.Parse(logObj["date"]?.ToString() ?? DateTime.Now.ToString()),
                            Comment = logObj["comment"]?.ToString() ?? string.Empty,
                            Difficulty = logObj["difficulty"]?.GetValue<int>() ?? 0,
                            Rating = logObj["rating"]?.GetValue<int>() ?? 0,
                            Duration = TimeSpan.Parse(logObj["duration"]?.ToString() ?? "00:00:00")
                        });
                    }

                    newTourWithLogs.Add(tourWithLogs);
                }

                TourWithLogs = newTourWithLogs;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void ExportToReport()
        {
            // TODO: Implement report generation
        }

        private void PrintList()
        {
            // TODO: Implement printing functionality
        }
    }
} 