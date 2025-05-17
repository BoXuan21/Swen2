using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.ViewModels
{
    public class ToursTabViewModel
    {
        public ICommand OpenCreatePopupCommand { get; }

        public event Action? RequestOpenCreatePopup;

        public ToursTabViewModel()
        {
            OpenCreatePopupCommand = new RelayCommand(OnOpenCreatePopup);
        }

        private void OnOpenCreatePopup()
        {
            RequestOpenCreatePopup?.Invoke();
        }
    }
}
