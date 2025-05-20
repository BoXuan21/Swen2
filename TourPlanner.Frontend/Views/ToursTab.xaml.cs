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

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for ToursTab.xaml
    /// </summary>
    public partial class ToursTab : UserControl
    {
        private ToursTabViewModel _viewModel;
        private TourApiClient _tourApiClient;
        private ObservableCollection<string> _tourNames;

        public ToursTab()
        {
            InitializeComponent();
            _tourApiClient = new TourApiClient();
            _tourNames = new ObservableCollection<string>();
            
            // Clear the ListBox at startup
            if (FindName("TourList") is ListBox tourList)
            {
                tourList.Items.Clear();
            }
            
            InitializeEventHandlers();
            LoadToursAsync();
        }

        private async void LoadToursAsync()
        {
            // Clear everything first
            _viewModel.TourNames.Clear();

            ConnectionStatus.Text = "Connecting to backend...";
            ConnectionStatus.Foreground = Brushes.Blue;

            try
            {
                var tours = await _tourApiClient.GetToursAsync();
                
                // Only add tours if we got a non-null response with tours
                if (tours != null && tours.Count > 0)
                {
                    foreach (var tour in tours)
                    {
                        _viewModel.TourNames.Add(tour["Name"].ToString());
                    }

                    ConnectionStatus.Text = "Connected to backend";
                    ConnectionStatus.Foreground = Brushes.Green;
                }
                else
                {
                    ConnectionStatus.Text = "No tours available";
                    ConnectionStatus.Foreground = Brushes.OrangeRed;
                }
            }
            catch (System.Exception ex)
            {
                ConnectionStatus.Text = "Error: Backend is offline";
                ConnectionStatus.Foreground = Brushes.Red;
                
                MessageBox.Show($"Cannot connect to the backend server: {ex.Message}\n\nPlease ensure the backend server is running and try again.", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadToursAsync();
        }

        private void InitializeEventHandlers()
        {
            _viewModel = new ToursTabViewModel();
            DataContext = _viewModel;

            // Create popup handler
            _viewModel.RequestOpenCreatePopup += () =>
            {
                var popup = new CreateTourPopup();
                var popupVm = new CreateTourPopupViewModel();
                popup.DataContext = popupVm;
                popupVm.RequestClose += () => 
                {
                    popup.Close();
                    LoadToursAsync(); // Refresh the tour list after creation
                };
                popup.ShowDialog();
            };

            // Delete popup handler
            _viewModel.RequestOpenDeletePopup += async (tourName) =>
            {
                if (string.IsNullOrEmpty(tourName))
                {
                    MessageBox.Show("Please select a tour to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ConnectionStatus.Text = "Loading tour details...";
                ConnectionStatus.Foreground = Brushes.Blue;
                
                try
                {
                    // Get all tours to find the ID
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
                            try
                            {
                                await _tourApiClient.DeleteTourAsync(tourId);
                                ConnectionStatus.Text = "Tour deleted successfully";
                                ConnectionStatus.Foreground = Brushes.Green;
                                LoadToursAsync(); // Refresh the tour list
                            }
                            catch (System.Exception ex)
                            {
                                ConnectionStatus.Text = "Error deleting tour";
                                ConnectionStatus.Foreground = Brushes.Red;
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
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            // Modify popup handler
            _viewModel.RequestOpenModifyPopup += async (tourName) =>
            {
                if (string.IsNullOrEmpty(tourName))
                {
                    MessageBox.Show("Please select a tour to modify.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ConnectionStatus.Text = "Loading tour details...";
                ConnectionStatus.Foreground = Brushes.Blue;
                try
                {
                    // Get all tours to find the ID
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
                        MessageBox.Show($"Could not find tour {tourName} to modify", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    string from = tourToModify["FromLocation"].ToString();
                    string to = tourToModify["ToLocation"].ToString();
                    string distance = tourToModify["Distance"].ToString();
                    string transportType = tourToModify["TransportType"].ToString();

                    var popup = new ModifyTourPopup();
                    var popupVm = new ModifyTourPopupViewModel(tourName, from, to, distance, transportType);
                    popup.DataContext = popupVm;
                    
                    popupVm.RequestClose += () => popup.Close();
                    popupVm.ModificationConfirmed += async (confirmed) =>
                    {
                        if (confirmed)
                        {
                            try
                            {
                                ConnectionStatus.Text = "Updating tour...";
                                ConnectionStatus.Foreground = Brushes.Blue;
                                
                                await _tourApiClient.UpdateTourAsync(
                                    tourId,
                                    popupVm.TourName,
                                    tourToModify["Description"]?.ToString() ?? "", // Keep existing description
                                    popupVm.From,
                                    popupVm.To,
                                    popupVm.TransportType,
                                    float.Parse(popupVm.Distance)
                                );
                                
                                ConnectionStatus.Text = "Tour updated successfully";
                                ConnectionStatus.Foreground = Brushes.Green;
                                LoadToursAsync(); // Refresh the tour list
                            }
                            catch (System.Exception ex)
                            {
                                ConnectionStatus.Text = "Error updating tour";
                                ConnectionStatus.Foreground = Brushes.Red;
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
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            // Find and hook up the tour list
            if (FindName("TourList") is ListBox tourList)
            {
                tourList.SelectionChanged += TourList_SelectionChanged;
            }
        }

        private async void TourList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is string tourName)
            {
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
                        
                        // Extract just the transport type value
                        string transportType = selectedTour["TransportType"]?.ToString() ?? "";
                        if (transportType.Contains("."))
                        {
                            transportType = transportType.Split('.').Last();
                        }
                        // Remove "ComboBoxItem" and ":" if present
                        transportType = transportType.Replace("ComboBoxItem", "").Replace(":", "").Trim();
                        _viewModel.SelectedTourTransportType = transportType;
                        
                        _viewModel.SelectedTourDistance = selectedTour["Distance"]?.ToString() ?? "";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error loading tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
