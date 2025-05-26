using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using System.Diagnostics;
using TourPlanner.Frontend.Utils;
using System.Text.Json.Nodes;

namespace TourPlanner.Frontend.Popups
{
    public partial class AddLogPopup : Window
    {
        private readonly AddLogPopupViewModel _viewModel;
        private readonly TourApiClient _apiClient;

        public AddLogPopup(TourViewModel selectedTour)
        {
            InitializeComponent();
            _apiClient = new TourApiClient();
            _viewModel = new AddLogPopupViewModel(selectedTour.Id);
            _viewModel.RequestClose += () => this.Close();
            DataContext = _viewModel;
        }

        public AddLogPopupViewModel ViewModel => (AddLogPopupViewModel)DataContext;

        /*private async void OnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.Comment))
                {
                    MessageBox.Show("Please enter a comment", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var duration = TimeSpan.FromHours(_viewModel.Hours) + TimeSpan.FromMinutes(_viewModel.Minutes);
                if (duration <= TimeSpan.Zero)
                {
                    MessageBox.Show("Duration must be greater than 0", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Debug.WriteLine($"Creating new log for tour {_viewModel.TourId}");
                var log = await _apiClient.CreateLogAsync(
                    _viewModel.TourId,
                    _viewModel.Comment,
                    _viewModel.Difficulty,
                    _viewModel.Rating,
                    duration
                );

                if (log != null)
                {
                    Debug.WriteLine($"Successfully created log with ID: {log["id"]?.ToString()}");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating log: {ex.Message}");
                MessageBox.Show($"Error creating log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }*/

        public class AddLogViewModel : INotifyPropertyChanged
        {
            private string _tourId = string.Empty;
            private DateTime _date;
            private string _comment = string.Empty;
            private int _difficulty;
            private int _rating;
            private int _hours;
            private int _minutes;

            public string TourId
            {
                get => _tourId;
                set
                {
                    _tourId = value;
                    OnPropertyChanged(nameof(TourId));
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

            public int Hours
            {
                get => _hours;
                set
                {
                    _hours = value;
                    OnPropertyChanged(nameof(Hours));
                }
            }

            public int Minutes
            {
                get => _minutes;
                set
                {
                    _minutes = value;
                    OnPropertyChanged(nameof(Minutes));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 