using TourPlanner.Models;
using TourPlanner.Data.Repositories;

namespace TourPlanner.Test.RepositoryTests
{
    public class ListsRepositoryTest
    {
        [Fact]
        public void ListsRepository_ShouldImplementInterface()
        {
            // Arrange & Act
            var repository = new ListsRepository();

            // Assert
            Assert.IsAssignableFrom<IListsRepository>(repository);
        }

        [Fact]
        public void List_Constructor_ShouldInitializeProperties()
        {
            // Act
            var list = new List();

            // Assert
            Assert.NotNull(list.Id);
            Assert.NotEmpty(list.Id);
            Assert.NotNull(list.Tours);
            Assert.True(Guid.TryParse(list.Id, out _));
        }
    }
} 