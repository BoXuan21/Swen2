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
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;

namespace TourPlanner.Frontend.Views
{
    /// <summary>
    /// Interaction logic for ListsPage.xaml
    /// </summary>
    public partial class ListsPage : UserControl
    {
        public ListsPage()
        {
            InitializeComponent();
            DataContext = new ListsViewModel();
        }
    }
}
