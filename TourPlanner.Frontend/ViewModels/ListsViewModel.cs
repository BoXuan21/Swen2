using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using TourPlanner.Frontend.ViewModels.Base;
using TourPlanner.Frontend.Services;
using TourPlanner.Frontend.Models;
using TourPlanner.Frontend.Utils;
using System.Linq;

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
        private ObservableCollection<TourWithLogs> _allTourWithLogs;
        private string _searchText;

        public ICommand ExportToReportCommand { get; }
        public ICommand PrintListCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public ObservableCollection<TourWithLogs> TourWithLogs
        {
            get => _tourWithLogs;
            set
            {
                _tourWithLogs = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterData();
            }
        }

        public ListsViewModel()
        {
            _apiClient = new TourApiClient();
            TourWithLogs = new ObservableCollection<TourWithLogs>();
            _allTourWithLogs = new ObservableCollection<TourWithLogs>();
            _searchText = string.Empty;

            ExportToReportCommand = new RelayCommand(ExportToReport);
            PrintListCommand = new RelayCommand(PrintList);
            RefreshCommand = new RelayCommand(LoadData);
            SearchCommand = new RelayCommand(FilterData);
            ClearSearchCommand = new RelayCommand(ClearSearch);

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

                _allTourWithLogs = newTourWithLogs;
                FilterData(); // Apply current filter after loading
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void FilterData()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                TourWithLogs = new ObservableCollection<TourWithLogs>(_allTourWithLogs);
                return;
            }

            var searchTerms = SearchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filteredTours = new ObservableCollection<TourWithLogs>();

            foreach (var tourWithLogs in _allTourWithLogs)
            {
                var tour = tourWithLogs.Tour;
                var matchingLogs = new List<TourLog>();

                // Check if tour matches search criteria
                bool tourMatches = searchTerms.All(term =>
                    tour.Name.ToLower().Contains(term) ||
                    tour.Description.ToLower().Contains(term) ||
                    tour.From.ToLower().Contains(term) ||
                    tour.To.ToLower().Contains(term) ||
                    tour.TransportType.ToLower().Contains(term));

                // Check logs for matches
                foreach (var log in tourWithLogs.Logs)
                {
                    bool logMatches = searchTerms.All(term =>
                        log.Comment.ToLower().Contains(term) ||
                        log.Date.ToString("d").ToLower().Contains(term) ||
                        log.Difficulty.ToString().Contains(term) ||
                        log.Rating.ToString().Contains(term));

                    if (logMatches)
                    {
                        matchingLogs.Add(log);
                    }
                }

                // Include tour if either tour matches or it has matching logs
                if (tourMatches || matchingLogs.Any())
                {
                    var filteredTourWithLogs = new TourWithLogs(tour);
                    
                    // If tour matches, include all logs; otherwise, include only matching logs
                    if (tourMatches)
                    {
                        foreach (var log in tourWithLogs.Logs)
                        {
                            filteredTourWithLogs.Logs.Add(log);
                        }
                    }
                    else
                    {
                        foreach (var log in matchingLogs)
                        {
                            filteredTourWithLogs.Logs.Add(log);
                        }
                    }

                    filteredTours.Add(filteredTourWithLogs);
                }
            }

            TourWithLogs = filteredTours;
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
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