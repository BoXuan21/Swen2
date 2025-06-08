using System;
using System.Windows;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using System.Diagnostics;
using System.ComponentModel;
using TourPlanner.Frontend.Models;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.Popups
{
    public partial class EditLogPopup : Window
    {
        private readonly EditLogViewModel _viewModel;

        public EditLogPopup(TourLog log)
        {
            InitializeComponent();
            _viewModel = new EditLogViewModel(log);
            _viewModel.RequestClose += () => 
            {
                DialogResult = _viewModel.IsSaved;
                Close();
            };
            DataContext = _viewModel;
        }
    }

    public class EditLogViewModel : INotifyPropertyChanged
    {
        private readonly TourApiClient _apiClient;
        private string _id = string.Empty;
        private string _tourId = string.Empty;
        private DateTime _date;
        private string _comment = string.Empty;
        private int _difficulty;
        private int _rating;
        private int _hours;
        private int _minutes;

        public bool IsSaved { get; private set; }

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? RequestClose;

        public EditLogViewModel(TourLog log)
        {
            _apiClient = new TourApiClient();
            
            // Initialize properties with the log data
            Id = log.Id;
            TourId = log.TourId;
            Date = log.Date;
            Comment = log.Comment;
            Difficulty = log.Difficulty - 1;
            Rating = log.Rating - 1;
            Hours = log.Duration.Hours;
            Minutes = log.Duration.Minutes;
            
            // Initialize commands
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private async void OnSave()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Comment))
                {
                    MessageBox.Show("Please enter a comment", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var duration = TimeSpan.FromHours(Hours) + TimeSpan.FromMinutes(Minutes);
                if (duration <= TimeSpan.Zero)
                {
                    MessageBox.Show("Duration must be greater than 0", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Debug.WriteLine($"Updating log {Id} for tour {TourId}");
                await _apiClient.UpdateLogAsync(
                    Id,
                    TourId,
                    Date,
                    Comment,
                    Difficulty + 1,
                    Rating + 1,
                    duration
                );

                Debug.WriteLine($"Successfully updated log {Id}");
                IsSaved = true;
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating log: {ex.Message}");
                MessageBox.Show($"Error updating log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel()
        {
            IsSaved = false;
            RequestClose?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 