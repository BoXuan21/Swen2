using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using TourPlaner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using TourPlanner.Frontend.Utils;
using TourPlanner.Services;
using TourPlanner.Services.Interfaces;

namespace TourPlanner.Frontend.ViewModels
{
    public class CreateTourPopupViewModel : INotifyPropertyChanged
    {
        private IToursService _toursService = new ToursService(new TourPlannerDbContext());
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

        private string _distance;
        public string Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        private string _transportType = "Car"; // Default value
        public string TransportType
        {
            get => _transportType;
            set
            {
                _transportType = value;
                OnPropertyChanged(nameof(TransportType));
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
            if (string.IsNullOrWhiteSpace(TourName))
            {
                MessageBox.Show("Please enter a tour name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(From))
            {
                MessageBox.Show("Please enter a starting location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(To))
            {
                MessageBox.Show("Please enter a destination.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Distance) || !float.TryParse(Distance, out float distanceValue))
            {
                MessageBox.Show("Please enter a valid distance value.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _toursService.AddTour(TourName, "", From, To, TransportType);

                // Close the dialog
                RequestClose?.Invoke();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Connection error: The tour planner server is not available or refused the connection. " +
                    $"The application will use local storage instead.\n\n" +
                    $"Technical details: {ex.Message}",
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Try again with mock data
                try
                {
                    

                    // Close the dialog
                    RequestClose?.Invoke();
                }
                catch (Exception innerEx)
                {
                    MessageBox.Show($"1An error occurred while creating the tour: {innerEx.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"2An error occurred while creating the tour: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            RequestClose?.Invoke();
        }

        private void OnCancel() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
