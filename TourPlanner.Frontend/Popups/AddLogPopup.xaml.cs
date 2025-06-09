using System.Windows;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;

namespace TourPlanner.Frontend.Popups
{
    public partial class AddLogPopup : Window
    {
        private readonly AddLogPopupViewModel _viewModel;
        public Action? OnLogAdded { get; set; }

        public AddLogPopup(TourViewModel selectedTour)
        {
            InitializeComponent();
            _viewModel = new AddLogPopupViewModel(selectedTour.Id);
            _viewModel.RequestClose += () => this.Close();
            _viewModel.LogAdded += () => this.OnLogAdded?.Invoke();
            DataContext = _viewModel;
        }


        public AddLogPopupViewModel ViewModel => (AddLogPopupViewModel)DataContext;
    }
} 