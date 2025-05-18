using System;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class ToursTabViewModel
    {
        public ICommand OpenCreatePopupCommand { get; }
        public ICommand OpenDeletePopupCommand { get; }
        public ICommand OpenModifyPopupCommand { get; }

        public event Action? RequestOpenCreatePopup;
        public event Action<string>? RequestOpenDeletePopup;
        public event Action<string>? RequestOpenModifyPopup;

        private string? _selectedTourName;
        public string? SelectedTourName
        {
            get => _selectedTourName;
            set
            {
                _selectedTourName = value;
                ((RelayCommand)OpenDeletePopupCommand).RaiseCanExecuteChanged();
                ((RelayCommand)OpenModifyPopupCommand).RaiseCanExecuteChanged();
            }
        }

        public ToursTabViewModel()
        {
            OpenCreatePopupCommand = new RelayCommand(OnOpenCreatePopup);
            OpenDeletePopupCommand = new RelayCommand(OnOpenDeletePopup, CanOpenDeletePopup);
            OpenModifyPopupCommand = new RelayCommand(OnOpenModifyPopup, CanOpenModifyPopup);
        }

        private void OnOpenCreatePopup()
        {
            RequestOpenCreatePopup?.Invoke();
        }

        private void OnOpenDeletePopup()
        {
            if (SelectedTourName != null)
            {
                RequestOpenDeletePopup?.Invoke(SelectedTourName);
            }
        }

        private void OnOpenModifyPopup()
        {
            if (SelectedTourName != null)
            {
                RequestOpenModifyPopup?.Invoke(SelectedTourName);
            }
        }

        private bool CanOpenDeletePopup()
        {
            return !string.IsNullOrEmpty(SelectedTourName);
        }

        private bool CanOpenModifyPopup()
        {
            return !string.IsNullOrEmpty(SelectedTourName);
        }
    }
}
