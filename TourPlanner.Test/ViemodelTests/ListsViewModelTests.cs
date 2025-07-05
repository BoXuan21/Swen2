using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Models;

public class ListsViewModelTests
{
    private ListsViewModel CreateViewModelWithTestData()
    {
        var vm = new ListsViewModel();

        // Manually set _allTourWithLogs and TourWithLogs for testing
        var tour1 = new Tour { Id = "1", Name = "Alps Adventure", Description = "Mountain tour", From = "Vienna", To = "Innsbruck", TransportType = "Car" };
        var tour2 = new Tour { Id = "2", Name = "City Trip", Description = "Urban exploration", From = "Graz", To = "Salzburg", TransportType = "Train" };

        var log1 = new TourLog { Id = "l1", TourId = "1", Comment = "Great view!", Date = new DateTime(2024, 1, 1), Difficulty = 3, Rating = 5, Duration = TimeSpan.FromHours(2) };
        var log2 = new TourLog { Id = "l2", TourId = "2", Comment = "Nice city", Date = new DateTime(2024, 2, 2), Difficulty = 2, Rating = 4, Duration = TimeSpan.FromHours(1) };

        var twl1 = new TourWithLogs(tour1);
        twl1.Logs.Add(log1);

        var twl2 = new TourWithLogs(tour2);
        twl2.Logs.Add(log2);

        // Use reflection to set the private _allTourWithLogs field
        var field = typeof(ListsViewModel).GetField("_allTourWithLogs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(vm, new ObservableCollection<TourWithLogs> { twl1, twl2 });

        // Also set TourWithLogs to match
        vm.TourWithLogs = new ObservableCollection<TourWithLogs> { twl1, twl2 };

        return vm;
    }

    [Fact]
    public void FilterData_EmptySearch_ReturnsAllTours()
    {
        var vm = CreateViewModelWithTestData();
        vm.SearchText = ""; // Triggers FilterData

        Assert.Equal(2, vm.TourWithLogs.Count);
    }

    [Fact]
    public void FilterData_SearchByTourName_FiltersCorrectly()
    {
        var vm = CreateViewModelWithTestData();
        vm.SearchText = "Alps"; // Should match only the first tour

        Assert.Single(vm.TourWithLogs);
        Assert.Equal("Alps Adventure", vm.TourWithLogs[0].Tour.Name);
    }

    [Fact]
    public void FilterData_SearchByLogComment_FiltersCorrectly()
    {
        var vm = CreateViewModelWithTestData();
        vm.SearchText = "city"; // Should match only the second tour via log comment

        Assert.Single(vm.TourWithLogs);
        Assert.Equal("City Trip", vm.TourWithLogs[0].Tour.Name);
        Assert.Single(vm.TourWithLogs[0].Logs);
        Assert.Equal("Nice city", vm.TourWithLogs[0].Logs[0].Comment);
    }

    [Fact]
    public void ClearSearch_ResetsSearchTextAndShowsAll()
    {
        var vm = CreateViewModelWithTestData();
        vm.SearchText = "Alps";
        Assert.Single(vm.TourWithLogs);

        // Use reflection to call private ClearSearch
        var method = typeof(ListsViewModel).GetMethod("ClearSearch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(vm, null);

        Assert.Equal("", vm.SearchText);
        Assert.Equal(2, vm.TourWithLogs.Count);
    }
}
