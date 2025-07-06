using System;
using System.Windows;
using TourPlanner.Frontend.ViewModels;
using TourPlanner.Frontend.Services;
using System.Diagnostics;
using System.ComponentModel;
using TourPlanner.Frontend.Models;
using System.Windows.Input;
using TourPlanner.Frontend.Utils;

namespace TourPlanner.Frontend.Popups
{
    public partial class EditLogPopup : Window
    {
        private readonly EditLogPopupViewModel _viewModel;

        public EditLogPopup(TourLog log)
        {
            InitializeComponent();
            _viewModel = new EditLogPopupViewModel(log);
            _viewModel.RequestClose += () => 
            {
                DialogResult = _viewModel.IsSaved;
                Close();
            };
            DataContext = _viewModel;
        }
    }

    
} 