using System.Windows.Controls;
using TourPlanner.Frontend.Popups;
using TourPlanner.Frontend.ViewModels;

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for ToursTab.xaml
    /// </summary>
    public partial class ToursTab : UserControl
    {
        public ToursTab()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            var viewModel = new ToursTabViewModel();
            DataContext = viewModel;

            // Invokes RequestOpenCreatePopup and executes functions in the brackets
            viewModel.RequestOpenCreatePopup += () =>
            {
                var popup = new CreateTourPopup(); // this is a Window
                var popupVm = new CreateTourPopupViewModel();
                popup.DataContext = popupVm;
                popupVm.RequestClose += () => popup.Close(); // Assigns function to the button to close the window when pressed
                popup.ShowDialog(); // Opens popup
            };

            // Find and hook up the tour list
            if (FindName("TourList") is ListBox tourList)
                tourList.SelectionChanged += TourList_SelectionChanged;
        }

        private void TourList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem != null)
            {
                // TODO: Update the tour details when a tour is selected
                // This will populate the General and Logs tabs with the selected tour's information
            }
        }
    }
}
