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
using System.Text.Json;

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

        private void LogsTab_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.LoadToursCommand.Execute(null);
        }

        private void OnRequestOpenAddLogPopup()
        {
            var popup = new Popups.AddLogPopup(_viewModel.SelectedTour!);
            if (popup.ShowDialog() == true)
            {
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            }
        }

        private void OnRequestOpenEditLogPopup(TourLog log)
        {
            var popup = new Popups.EditLogPopup(log);
            if (popup.ShowDialog() == true)
            {
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            }
        }

        private async void OnRequestOpenDeleteLogPopup(TourLog log)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this log?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiClient.DeleteLogAsync(log.Id);
                    _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting log: {ex.Message}");
                    MessageBox.Show(
                        $"Error deleting log: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("TourComboBox_SelectionChanged called");
            if (_viewModel.LoadLogsCommand.CanExecute(_viewModel.SelectedTour?.Id))
            {
                Debug.WriteLine($"Tour selected: {_viewModel.SelectedTour.Name} (ID: {_viewModel.SelectedTour.Id})");
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            }
            else
            {
                Debug.WriteLine("No tour selected");
                _viewModel.Logs.Clear();
            }
        }
    }
}
