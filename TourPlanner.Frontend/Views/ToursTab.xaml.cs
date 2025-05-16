using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            // Find and hook up the buttons
            if (FindName("CreateButton") is Button createButton)
                createButton.Click += CreateButton_Click;
            if (FindName("DeleteButton") is Button deleteButton)
                deleteButton.Click += DeleteButton_Click;
            if (FindName("ModifyButton") is Button modifyButton)
                modifyButton.Click += ModifyButton_Click;

            // Find and hook up the tour list
            if (FindName("TourList") is ListBox tourList)
                tourList.SelectionChanged += TourList_SelectionChanged;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show create tour dialog
            MessageBox.Show("Create Tour Dialog will be shown here", "Create Tour");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement delete functionality
            if (MessageBox.Show("Are you sure you want to delete this tour?", "Delete Tour",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Implement delete logic here
            }
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show modify tour dialog
            MessageBox.Show("Modify Tour Dialog will be shown here", "Modify Tour");
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
