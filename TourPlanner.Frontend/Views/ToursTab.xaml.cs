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

            // Create popup handler
            viewModel.RequestOpenCreatePopup += () =>
            {
                var popup = new CreateTourPopup();
                var popupVm = new CreateTourPopupViewModel();
                popup.DataContext = popupVm;
                popupVm.RequestClose += () => popup.Close();
                popup.ShowDialog();
            };

            // Delete popup handler
            viewModel.RequestOpenDeletePopup += (tourName) =>
            {
                var popup = new DeleteTourPopup();
                var popupVm = new DeleteTourPopupViewModel(tourName);
                popup.DataContext = popupVm;
                
                popupVm.RequestClose += () => popup.Close();
                popupVm.DeleteConfirmed += (confirmed) =>
                {
                    if (confirmed)
                    {
                        // TODO: Implement actual delete logic here
                        // viewModel.DeleteTour(tourName);
                    }
                };
                
                popup.ShowDialog();
            };

            // Modify popup handler
            viewModel.RequestOpenModifyPopup += (tourName) =>
            {
                // TODO: Get the actual tour data here
                string from = "Current From Location";
                string to = "Current To Location";
                string distance = "Current Distance";
                string transportType = "Car";

                var popup = new ModifyTourPopup();
                var popupVm = new ModifyTourPopupViewModel(tourName, from, to, distance, transportType);
                popup.DataContext = popupVm;
                
                popupVm.RequestClose += () => popup.Close();
                popupVm.ModificationConfirmed += (confirmed) =>
                {
                    if (confirmed)
                    {
                        // TODO: Implement actual modify logic here
                        // viewModel.ModifyTour(popupVm.TourName, popupVm.From, popupVm.To, popupVm.Distance, popupVm.TransportType);
                    }
                };
                
                popup.ShowDialog();
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
