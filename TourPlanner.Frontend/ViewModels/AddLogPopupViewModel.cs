using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Frontend.Services;
using TourPlanner.Frontend.Utils;
using System.Diagnostics;

namespace TourPlanner.Frontend.ViewModels
{
    public class AddLogPopupViewModel : INotifyPropertyChanged
    {
        private readonly TourApiClient _tourApiClient;
        private string _comment = string.Empty;
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        private int _difficulty;
        public int Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        private int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        private string _hours = "0";
        public string Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                OnPropertyChanged(nameof(Hours));
            }
        }

        private string _minutes = "0";
        public string Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                OnPropertyChanged(nameof(Minutes));
            }
        }

        public string TourId { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? RequestClose;

        public AddLogPopupViewModel(string tourId)
        {
            _tourApiClient = new TourApiClient();
            TourId = tourId;
            SaveCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private async void OnAdd()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(Comment))
            {
                MessageBox.Show("Please enter a comment.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Difficulty < 0 || Difficulty > 4)
            {
                MessageBox.Show("Difficulty must be between 0 and 4.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Rating < 0 || Rating > 4)
            {
                MessageBox.Show("Rating must be between 0 and 4.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(Hours, out int hours) || hours < 0)
            {
                MessageBox.Show("Please enter a valid number of hours.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(Minutes, out int minutes) || minutes < 0 || minutes > 59)
            {
                MessageBox.Show("Please enter a valid number of minutes (0-59).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var duration = new TimeSpan(hours, minutes, 0);
                Debug.WriteLine($"Creating log with values: TourId={TourId}, Comment={Comment}, Difficulty={Difficulty + 1}, Rating={Rating + 1}, Duration={duration}");
                
                await _tourApiClient.CreateLogAsync(TourId, Comment, Difficulty + 1, Rating + 1, duration);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel()
        {
            RequestClose?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 