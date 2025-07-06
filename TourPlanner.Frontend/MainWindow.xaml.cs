using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.Frontend.Views;

namespace TourPlanner.Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkMode = false;

        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme("Themes/LightTheme.xaml"); // Default to light
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            var themePath = _isDarkMode ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
            ApplyTheme(themePath);
        }

        private void ApplyTheme(string resourcePath)
        {
            var uri = new Uri(resourcePath, UriKind.Relative);
            var resourceDict = new ResourceDictionary { Source = uri };

            // Clear current merged dictionaries and apply new one
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}