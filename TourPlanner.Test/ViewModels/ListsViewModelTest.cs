using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Models;

namespace TourPlanner.Test.ViewModels
{
    public class ListsViewModelTest
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var viewModel = new ListsViewModel();

            // Assert
            Assert.NotNull(viewModel.TourWithLogs);
            Assert.NotNull(viewModel.ExportToReportCommand);
            Assert.NotNull(viewModel.PrintListCommand);
            Assert.NotNull(viewModel.RefreshCommand);
        }

        [Fact]
        public void TourWithLogs_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var tour = new Tour
            {
                Id = "test-id",
                Name = "Test Tour",
                From = "Vienna",
                To = "Salzburg"
            };

            // Act
            var tourWithLogs = new TourWithLogs(tour);

            // Assert
            Assert.Equal(tour, tourWithLogs.Tour);
            Assert.NotNull(tourWithLogs.Logs);
            Assert.Empty(tourWithLogs.Logs);
        }
    }
} 