using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TourPlanner.Frontend.Models;
using TourPlanner.Frontend.Services;
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

    public class ListsViewModel : INotifyPropertyChanged
    {
        private readonly TourApiClient _apiClient;
        private readonly DispatcherTimer _refreshTimer;
        private ObservableCollection<TourWithLogs> _tourWithLogs;
        private ObservableCollection<TourWithLogs> _allTourWithLogs;
        private string _searchText;

        public event PropertyChangedEventHandler? PropertyChanged;

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
                OnPropertyChanged(nameof(TourWithLogs));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
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
                        EstimatedTime = TimeSpan.FromSeconds(tourObj["estimatedTime"]?.GetValue<double>() ?? 0),
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
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"TourReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };
                if (saveFileDialog.ShowDialog() != true)
                    return;

                string filePath = Path.GetFullPath(saveFileDialog.FileName);

                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Tour Report";

                GlobalFontSettings.UseWindowsFontsUnderWindows = true;

                // Create font objects
                XFont titleFont = new XFont("Arial", 20);
                XFont headerFont = new XFont("Arial", 14);
                XFont normalFont = new XFont("Arial", 12);
                XFont italicFont = new XFont("Arial", 12);

                // Add a page
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                double y = 40;
                double leftMargin = 40;
                double rightMargin = 40;
                double pageWidth = page.Width - leftMargin - rightMargin;

                // Title
                gfx.DrawString("Tour Report", titleFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 30), XStringFormats.TopCenter);
                y += 35;
                gfx.DrawString($"Generated: {DateTime.Now:g}", normalFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                y += 30;

                foreach (var tourWithLogs in TourWithLogs)
                {
                    var tour = tourWithLogs.Tour;

                    // Add new page if needed
                    if (y > page.Height - 100)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }

                    gfx.DrawString(tour.Name, headerFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                    y += 22;
                    gfx.DrawString($"Description: {tour.Description}", normalFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                    y += 18;
                    gfx.DrawString($"From: {tour.From}    To: {tour.To}    Transport: {tour.TransportType}", normalFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                    y += 18;
                    gfx.DrawString($"Distance: {tour.Distance / 1000.0:F2} km    Estimated Time: {tour.EstimatedTime:hh\\:mm\\:ss}", normalFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                    y += 18;

                    // Logs
                    if (tourWithLogs.Logs.Any())
                    {
                        gfx.DrawString("Logs:", normalFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                        y += 18;

                        // Table header
                        string[] headers = { "Date", "Duration", "Difficulty", "Rating", "Comment" };
                        double[] colWidths = { 70, 70, 60, 60, pageWidth - 260 };
                        double x = leftMargin;
                        for (int i = 0; i < headers.Length; i++)
                        {
                            gfx.DrawString(headers[i], normalFont, XBrushes.Black, new XRect(x, y, colWidths[i], 20), XStringFormats.TopLeft);
                            x += colWidths[i];
                        }
                        y += 16;

                        // Table rows
                        foreach (var log in tourWithLogs.Logs)
                        {
                            x = leftMargin;
                            string[] row = {
                                log.Date.ToString("yyyy-MM-dd"),
                                log.Duration.ToString(@"hh\:mm\:ss"),
                                log.Difficulty.ToString(),
                                log.Rating.ToString(),
                                log.Comment ?? ""
                            };
                            for (int i = 0; i < row.Length; i++)
                            {
                                gfx.DrawString(row[i], normalFont, XBrushes.Black, new XRect(x, y, colWidths[i], 20), XStringFormats.TopLeft);
                                x += colWidths[i];
                            }
                            y += 16;

                            // Add new page if needed
                            if (y > page.Height - 60)
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                y = 40;
                            }
                        }
                    }
                    else
                    {
                        gfx.DrawString("No logs available.", italicFont, XBrushes.Black, new XRect(leftMargin, y, pageWidth, 20), XStringFormats.TopLeft);
                        y += 18;
                    }

                    y += 20; // Spacer
                }

                document.Save(filePath);
                document.Close();

                System.Windows.MessageBox.Show("PDF export succeeded!", "Success");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to export report: {ex.Message}\n{ex.StackTrace}",
                    "Export Error"
                );
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void PrintList()
        {
            // TODO: Implement printing functionality
        }
    }
}