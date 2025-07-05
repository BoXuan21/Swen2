using System.Collections.ObjectModel;
using System.Windows.Controls;
using TourPlanner.Frontend.Popups;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json.Nodes;
using System.Windows.Media;
using System.Linq;
using log4net;

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for ToursTab.xaml
    /// </summary>
    public partial class ToursTab : UserControl
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ToursTab));

        private ToursTabViewModel _viewModel;
        private TourApiClient _tourApiClient;
        private ObservableCollection<string> _tourNames;

        public ToursTab()
        {
            InitializeComponent();
            _tourApiClient = new TourApiClient();
            _tourNames = new ObservableCollection<string>();

            if (FindName("TourList") is ListBox tourList)
            {
                tourList.Items.Clear();
            }

            _logger.Info("ToursTab initialized.");

            InitializeEventHandlers();
            LoadToursAsync();
        }

        private async void LoadToursAsync()
        {
            _viewModel.TourNames.Clear();

            ConnectionStatus.Text = "Connecting to backend...";
            ConnectionStatus.Foreground = Brushes.Blue;

            _logger.Info("Attempting to load tours from backend.");

            try
            {
                var tours = await _tourApiClient.GetToursAsync();

                if (tours != null && tours.Count > 0)
                {
                    foreach (var tour in tours)
                    {
                        _viewModel.TourNames.Add(tour["Name"].ToString());
                    }

                    ConnectionStatus.Text = "Connected to backend";
                    ConnectionStatus.Foreground = Brushes.Green;

                    _logger.Info($"Loaded {tours.Count} tours from backend.");
                }
                else
                {
                    ConnectionStatus.Text = "No tours available";
                    ConnectionStatus.Foreground = Brushes.OrangeRed;

                    _logger.Warn("No tours received from backend.");
                }
            }
            catch (System.Exception ex)
            {
                ConnectionStatus.Text = "Error: Backend is offline";
                ConnectionStatus.Foreground = Brushes.Red;

                _logger.Error("Exception while loading tours.", ex);

                MessageBox.Show($"Cannot connect to the backend server: {ex.Message}\n\nPlease ensure the backend server is running and try again.",
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.Info("Refresh button clicked.");
            LoadToursAsync();
        }

        private void InitializeEventHandlers()
        {
            _viewModel = new ToursTabViewModel();
            DataContext = _viewModel;

            _viewModel.RequestOpenCreatePopup += () =>
            {
                _logger.Info("Opening CreateTourPopup.");
                var popup = new CreateTourPopup();
                var popupVm = new CreateTourPopupViewModel();
                popup.DataContext = popupVm;
                popupVm.RequestClose += () =>
                {
                    popup.Close();
                    _logger.Info("CreateTourPopup closed. Reloading tours.");
                    LoadToursAsync();
                };
                popup.ShowDialog();
            };

            _viewModel.RequestOpenDeletePopup += async (tourName) =>
            {
                _logger.Info($"Request to delete tour: {tourName}");

                if (string.IsNullOrEmpty(tourName))
                {
                    MessageBox.Show("Please select a tour to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ConnectionStatus.Text = "Loading tour details...";
                ConnectionStatus.Foreground = Brushes.Blue;

                try
                {
                    var tours = await _tourApiClient.GetToursAsync();
                    string tourId = null;

                    foreach (var tour in tours)
                    {
                        if (tour["Name"].ToString() == tourName)
                        {
                            tourId = tour["Id"].ToString();
                            break;
                        }
                    }

                    if (tourId == null)
                    {
                        ConnectionStatus.Text = "Error: Tour not found";
                        ConnectionStatus.Foreground = Brushes.Red;

                        _logger.Warn($"Tour '{tourName}' not found for deletion.");
                        MessageBox.Show($"Could not find tour {tourName} to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var popup = new DeleteTourPopup();
                    var popupVm = new DeleteTourPopupViewModel(tourName);
                    popup.DataContext = popupVm;

                    popupVm.RequestClose += () => popup.Close();
                    popupVm.DeleteConfirmed += async (confirmed) =>
                    {
                        if (confirmed)
                        {
                            ConnectionStatus.Text = "Deleting tour...";
                            ConnectionStatus.Foreground = Brushes.Blue;

                            _logger.Info($"Deleting tour with ID: {tourId}");

                            try
                            {
                                await _tourApiClient.DeleteTourAsync(tourId);
                                ConnectionStatus.Text = "Tour deleted successfully";
                                ConnectionStatus.Foreground = Brushes.Green;

                                _logger.Info("Tour deleted successfully.");
                                LoadToursAsync();
                            }
                            catch (System.Exception ex)
                            {
                                ConnectionStatus.Text = "Error deleting tour";
                                ConnectionStatus.Foreground = Brushes.Red;

                                _logger.Error("Error deleting tour.", ex);
                                MessageBox.Show($"Error deleting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };

                    popup.ShowDialog();
                }
                catch (System.Exception ex)
                {
                    ConnectionStatus.Text = "Error loading tour details";
                    ConnectionStatus.Foreground = Brushes.Red;

                    _logger.Error("Error loading tour details for deletion.", ex);
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            _viewModel.RequestOpenModifyPopup += async (tourName) =>
            {
                _logger.Info($"Request to modify tour: {tourName}");

                if (string.IsNullOrEmpty(tourName))
                {
                    MessageBox.Show("Please select a tour to modify.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ConnectionStatus.Text = "Loading tour details...";
                ConnectionStatus.Foreground = Brushes.Blue;

                try
                {
                    var tours = await _tourApiClient.GetToursAsync();
                    JsonObject tourToModify = null;
                    string tourId = null;

                    foreach (var tour in tours)
                    {
                        if (tour["Name"].ToString() == tourName)
                        {
                            tourToModify = tour;
                            tourId = tour["Id"].ToString();
                            break;
                        }
                    }

                    if (tourToModify == null || tourId == null)
                    {
                        ConnectionStatus.Text = "Error: Tour not found";
                        ConnectionStatus.Foreground = Brushes.Red;

                        _logger.Warn($"Tour '{tourName}' not found for modification.");
                        MessageBox.Show($"Could not find tour {tourName} to modify", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string from = tourToModify["FromLocation"].ToString();
                    string to = tourToModify["ToLocation"].ToString();
                    string transportType = tourToModify["TransportType"].ToString();

                    var popup = new ModifyTourPopup();
                    var popupVm = new ModifyTourPopupViewModel(tourName, from, to, transportType);
                    popup.DataContext = popupVm;

                    popupVm.RequestClose += () => popup.Close();
                    popupVm.ModificationConfirmed += async (confirmed) =>
                    {
                        if (confirmed)
                        {
                            ConnectionStatus.Text = "Updating tour...";
                            ConnectionStatus.Foreground = Brushes.Blue;

                            _logger.Info($"Updating tour ID {tourId}");

                            try
                            {
                                await _tourApiClient.UpdateTourAsync(
                                    tourId,
                                    popupVm.TourName,
                                    tourToModify["Description"]?.ToString() ?? "",
                                    popupVm.From,
                                    popupVm.To,
                                    popupVm.TransportType,
                                    float.Parse(tourToModify["Distance"].ToString())
                                );

                                ConnectionStatus.Text = "Tour updated successfully";
                                ConnectionStatus.Foreground = Brushes.Green;

                                _logger.Info("Tour updated successfully.");
                                LoadToursAsync();
                            }
                            catch (System.Exception ex)
                            {
                                ConnectionStatus.Text = "Error updating tour";
                                ConnectionStatus.Foreground = Brushes.Red;

                                _logger.Error("Error updating tour.", ex);
                                MessageBox.Show($"Error updating tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };

                    popup.ShowDialog();
                }
                catch (System.Exception ex)
                {
                    ConnectionStatus.Text = "Error loading tour details";
                    ConnectionStatus.Foreground = Brushes.Red;

                    _logger.Error("Error loading tour details for modification.", ex);
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            if (FindName("TourList") is ListBox tourList)
            {
                tourList.SelectionChanged += TourList_SelectionChanged;
            }
        }

        private async void TourList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is string tourName)
            {
                _logger.Info($"Tour selected from list: {tourName}");

                try
                {
                    var tours = await _tourApiClient.GetToursAsync();
                    var selectedTour = tours.FirstOrDefault(t => t["Name"].ToString() == tourName);

                    if (selectedTour != null)
                    {
                        _viewModel.SelectedTourName = tourName;
                        _viewModel.SelectedTourDescription = selectedTour["Description"]?.ToString() ?? "";
                        _viewModel.SelectedTourFrom = selectedTour["FromLocation"]?.ToString() ?? "";
                        _viewModel.SelectedTourTo = selectedTour["ToLocation"]?.ToString() ?? "";

                        string transportType = selectedTour["TransportType"]?.ToString() ?? "";
                        if (transportType.Contains("."))
                        {
                            transportType = transportType.Split('.').Last();
                        }
                        transportType = transportType.Replace("ComboBoxItem", "").Replace(":", "").Trim();
                        _viewModel.SelectedTourTransportType = transportType;

                        _viewModel.SelectedTourEstimatedTime = FormatSecondsToHoursMinutes(selectedTour["EstimatedTime"]?.ToString());
                        _viewModel.SelectedTourDistance = FormatMetersToKilometers(selectedTour["Distance"]?.ToString());

                        if (RouteMap != null)
                        {
                            await RouteMap.SetRouteAsync(
                                _viewModel.SelectedTourFrom,
                                _viewModel.SelectedTourTo,
                                _viewModel.SelectedTourTransportType.ToLower()
                            );
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.Error("Error loading selected tour details.", ex);
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string FormatSecondsToHoursMinutes(string? secondsStr)
        {
            if (int.TryParse(secondsStr, out int seconds))
            {
                int hours = seconds / 3600;
                int minutes = (seconds % 3600) / 60;
                return $"{hours}:{minutes:D2}";
            }
            return secondsStr ?? "";
        }

        private string FormatMetersToKilometers(string? metersStr)
        {
            if (double.TryParse(metersStr, out double meters))
            {
                double kilometers = meters / 1000.0;
                return $"{kilometers:0.00} km";
            }
            return metersStr ?? "";
        }
    }
}