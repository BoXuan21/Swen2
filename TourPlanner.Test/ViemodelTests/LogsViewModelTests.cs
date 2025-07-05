using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using Xunit;
using Moq;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Models;
using TourPlanner.Frontend.Services;
using System.Text.Json.Nodes;
using System.Windows.Input;

public class LogsViewModelTests
{

    [Fact]
    public void AddLogCommand_CanExecute_ReturnsFalse_WhenNoTourSelected()
    {
        var vm = new LogsViewModel();
        Assert.False(vm.AddLogCommand.CanExecute(null));
    }

    [Fact]
    public void AddLogCommand_CanExecute_ReturnsTrue_WhenTourSelected()
    {
        var vm = new LogsViewModel();
        vm.SelectedTour = new TourViewModel { Id = "1", Name = "Tour 1" };
        Assert.True(vm.AddLogCommand.CanExecute(null));
    }

    [Fact]
    public void EditLogCommand_CanExecute_ReturnsFalse_WhenNoLogSelected()
    {
        var vm = new LogsViewModel();
        Assert.False(vm.EditLogCommand.CanExecute(null));
    }

    [Fact]
    public void EditLogCommand_CanExecute_ReturnsTrue_WhenLogSelected()
    {
        var vm = new LogsViewModel();
        vm.SelectedLog = new TourLog { Id = "log1" };
        Assert.True(vm.EditLogCommand.CanExecute(null));
    }

    [Fact]
    public void DeleteLogCommand_CanExecute_ReturnsFalse_WhenNoLogSelected()
    {
        var vm = new LogsViewModel();
        Assert.False(vm.DeleteLogCommand.CanExecute(null));
    }

    [Fact]
    public void DeleteLogCommand_CanExecute_ReturnsTrue_WhenLogSelected()
    {
        var vm = new LogsViewModel();
        vm.SelectedLog = new TourLog { Id = "log1" };
        Assert.True(vm.DeleteLogCommand.CanExecute(null));
    }

    [Fact]
    public void Setting_SelectedTour_RaisesPropertyChanged()
    {
        var vm = new LogsViewModel();
        bool raised = false;
        vm.PropertyChanged += (s, e) => { if (e.PropertyName == "SelectedTour") raised = true; };
        vm.SelectedTour = new TourViewModel { Id = "1", Name = "Tour 1" };
        Assert.True(raised);
    }

    [Fact]
    public void Setting_SelectedLog_RaisesPropertyChanged()
    {
        var vm = new LogsViewModel();
        bool raised = false;
        vm.PropertyChanged += (s, e) => { if (e.PropertyName == "SelectedLog") raised = true; };
        vm.SelectedLog = new TourLog { Id = "log1" };
        Assert.True(raised);
    }

    [Fact]
    public void OnAddLog_Raises_RequestOpenAddLogPopup_Event()
    {
        var vm = new LogsViewModel();
        bool eventRaised = false;
        vm.SelectedTour = new TourViewModel { Id = "1", Name = "Tour 1" };
        vm.RequestOpenAddLogPopup += () => eventRaised = true;
        vm.AddLogCommand.Execute(null);
        Assert.True(eventRaised);
    }

    [Fact]
    public void OnEditLog_Raises_RequestOpenEditLogPopup_Event()
    {
        var vm = new LogsViewModel();
        bool eventRaised = false;
        var log = new TourLog { Id = "log1" };
        vm.SelectedLog = log;
        vm.RequestOpenEditLogPopup += (l) => { if (l == log) eventRaised = true; };
        vm.EditLogCommand.Execute(null);
        Assert.True(eventRaised);
    }

    [Fact]
    public void OnDeleteLog_Raises_RequestOpenDeleteLogPopup_Event()
    {
        var vm = new LogsViewModel();
        bool eventRaised = false;
        var log = new TourLog { Id = "log1" };
        vm.SelectedLog = log;
        vm.RequestOpenDeleteLogPopup += (l) => { if (l == log) eventRaised = true; };
        vm.DeleteLogCommand.Execute(null);
        Assert.True(eventRaised);
    }
}
