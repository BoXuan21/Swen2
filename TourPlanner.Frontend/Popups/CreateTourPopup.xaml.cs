using System.Windows;
using TourPlanner.Frontend.ViewModels;

namespace TourPlanner.Frontend.Popups
{
    public partial class CreateTourPopup : Window
    {
        public CreateTourPopup()
        {
            InitializeComponent();

            var vm = new CreateTourPopupViewModel();
            // vm.RequestClose += () => this.DialogResult = true;
            this.DataContext = vm;
        }

        public CreateTourPopupViewModel ViewModel => (CreateTourPopupViewModel)DataContext;
    }
}
