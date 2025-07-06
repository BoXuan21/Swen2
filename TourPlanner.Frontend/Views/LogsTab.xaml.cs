using System;
using System.Windows;
using System.Windows.Controls;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using TourPlanner.Frontend.Popups;
using System.Threading.Tasks;
using TourPlanner.Frontend.Models;
using log4net;

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogsTab));

        private readonly LogsViewModel _viewModel;
        private readonly TourApiClient _apiClient;

        public LogsTab()
        {
            InitializeComponent();
            _logger.Info("Initializing LogsTab...");

            _apiClient = new TourApiClient();
            _viewModel = new LogsViewModel();
            DataContext = _viewModel;

            _viewModel.RequestOpenAddLogPopup += OnRequestOpenAddLogPopup;
            _viewModel.RequestOpenEditLogPopup += OnRequestOpenEditLogPopup;
            _viewModel.RequestOpenDeleteLogPopup += OnRequestOpenDeleteLogPopup;

            Loaded += LogsTab_Loaded;

            _logger.Info("LogsTab initialized.");
        }

        private void LogsTab_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.Info("LogsTab loaded - executing LoadToursCommand.");
            _viewModel.LoadToursCommand.Execute(null);
        }

        private void OnRequestOpenAddLogPopup()
        {
            _logger.Info("Opening AddLogPopup...");
            var popup = new AddLogPopup(_viewModel.SelectedTour!);
            popup.OnLogAdded = () =>
            {
                _logger.Info("Log added successfully.");
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            };

            if (popup.ShowDialog() == true)
            {
                _logger.Info("AddLogPopup closed with success.");
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            }
            else
            {
                _logger.Info("AddLogPopup closed without adding log.");
            }
        }

        private void OnRequestOpenEditLogPopup(TourLog log)
        {
            _logger.Info($"Opening EditLogPopup for log ID: {log.Id}");
            var popup = new EditLogPopup(log);
            if (popup.ShowDialog() == true)
            {
                _logger.Info($"Log edited: {log.Id}");
                _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
            }
            else
            {
                _logger.Info("EditLogPopup closed without saving changes.");
            }
        }

        private async void OnRequestOpenDeleteLogPopup(TourLog log)
        {
            _logger.Info($"Request to delete log ID: {log.Id}");

            var result = MessageBox.Show(
                "Are you sure you want to delete this log?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _logger.Info($"Deleting log ID: {log.Id}");
                    await _apiClient.DeleteLogAsync(log.Id);
                    _logger.Info($"Log deleted: {log.Id}");

                    _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error deleting log {log.Id}: {ex.Message}", ex);
                    MessageBox.Show(
                        $"Error deleting log: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                _logger.Info("Delete log operation canceled by user.");
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _logger.Info("TourComboBox_SelectionChanged triggered.");

            if (_viewModel.SelectedTour != null)
            {
                _logger.Info($"Selected tour: {_viewModel.SelectedTour.Name} (ID: {_viewModel.SelectedTour.Id})");

                if (_viewModel.LoadLogsCommand.CanExecute(_viewModel.SelectedTour?.Id))
                {
                    _viewModel.LoadLogsCommand.Execute(_viewModel.SelectedTour?.Id);
                }
                else
                {
                    _logger.Warn("LoadLogsCommand cannot execute for selected tour.");
                    _viewModel.Logs.Clear();
                }
            }
            else
            {
                _logger.Warn("No tour selected in ComboBox.");
                _viewModel.Logs.Clear();
            }
        }
    }
}
