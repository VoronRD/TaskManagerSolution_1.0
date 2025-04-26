using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Calendar.ViewModels
{
    public class CalendarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDataService _dataService;

        private DateTime _selectedDate = DateTime.Today;
        private List<Tasks> _allTasks = new();

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
                LoadTasksForDate(value);
            }
        }

        private DateTime _currentDate = DateTime.Today;
        public DateTime CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        private ObservableCollection<Tasks> _dailyPlans = new();
        public ObservableCollection<Tasks> DailyPlans
        {
            get => _dailyPlans;
            set
            {
                SetProperty(ref _dailyPlans, value);
                RaisePropertyChanged(nameof(HasPlans));
            }
        }

        public bool HasPlans => DailyPlans?.Any() == true;

        public ICommand RedactCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public CalendarViewModel(IRegionManager regionManager, IDataService dataService)
        {
            _regionManager = regionManager;
            _dataService = dataService;

            RedactCommand = new DelegateCommand(NavigateToTaskList);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);

            LoadAllTasks();
            LoadTasksForDate(SelectedDate);
        }

        private void NavigateToTaskList()
        {
            // Переход на TaskListView с передачей выбранной даты
            var parameters = new NavigationParameters
            {
                { "selectedDate", SelectedDate }
            };
            _regionManager.RequestNavigate("MainRegion", "TaskListView", parameters);
        }

        //private void OnRedact()
        //{
        //    System.Diagnostics.Debug.WriteLine("Редактирование задач");
        //}

        private void OnOpenSettings()
        {
            System.Diagnostics.Debug.WriteLine("Открытие настроек");
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private async void LoadAllTasks()
        {
            IsLoading = true;
            try
            {
                await Task.Run(() => _allTasks = _dataService.LoadTasks());
            }
            finally
            {
                IsLoading = false;
                RaisePropertyChanged(nameof(HasPlans));
            }
        }

        private void LoadTasksForDate(DateTime date)
        {
            var tasksForDate = _allTasks
                .Where(t => t.Deadline.Date == date.Date)
                .ToList();

            DailyPlans = new ObservableCollection<Tasks>(tasksForDate);
        }
    }


}