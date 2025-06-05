using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Calendar.ViewModels
{
    public class CalendarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;

        private DateTime _selectedDate = DateTime.Today;
        private List<Tasks> _allTasks = new();

        public DelegateCommand ShowAllTasksCommand { get; }

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

        public ObservableCollection<DateTime> DatesWithTasks { get; } = new ObservableCollection<DateTime>();

        public ICommand RedactCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public CalendarViewModel(IRegionManager regionManager, IDataService dataService, INotificationService notificationService,
            ISettingsService settingsService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _dataService = dataService;
            _notificationService = notificationService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;

            ShowAllTasksCommand = new DelegateCommand(NavigateToAllTasks);

            RedactCommand = new DelegateCommand(NavigateToTaskList);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);

            _eventAggregator.GetEvent<TaskChangedEvent>().Subscribe(OnTaskUpdated);
            _eventAggregator.GetEvent<TaskDeletedEvent>().Subscribe(OnTaskDeleted);

            LoadAllTasks();
            LoadTasksForDate(SelectedDate);
        }

        private void NavigateToAllTasks()
        {
            _regionManager.RequestNavigate("MainRegion", "TaskListView");
        }

        private void NavigateToTaskList()
        {
            var parameters = new NavigationParameters
            {
                { "selectedDate", SelectedDate }
            };
            _regionManager.RequestNavigate("MainRegion", "TaskListView", parameters);
        }

        private void OnTaskUpdated(Tasks updatedTask)
        {
            RefreshTasks();
        }

        private void OnTaskDeleted(Tasks deletedTask)
        {
            RefreshTasks();
        }

        private void RefreshTasks()
        {
            _allTasks = _dataService.LoadTasks();
            UpdateDatesWithTasks();
            LoadTasksForDate(SelectedDate);
        }

        private void UpdateDatesWithTasks()
        {
            var dates = _allTasks.Select(t => t.Deadline.Date).Distinct().ToList();
            DatesWithTasks.Clear();
            foreach (var date in dates)
                DatesWithTasks.Add(date);
        }

        private void OnOpenSettings()
        {
            _regionManager.RequestNavigate("MainRegion", "SettingsView");
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
                await System.Threading.Tasks.Task.Run(() =>
                {
                    _allTasks = _dataService.LoadTasks();
                    UpdateDatesWithTasks();
                });
                int days = _settingsService.GetNotificationDays();
                _notificationService.CheckUpcomingDeadlines(_allTasks, days);
            }
            finally
            {
                IsLoading = false;
                RaisePropertyChanged(nameof(HasPlans));
            }
        }

        private void LoadTasksForDate(DateTime date)
        {
            var tasksForDate = _allTasks.Where(t => t.Deadline.Date == date.Date).ToList();
            DailyPlans = new ObservableCollection<Tasks>(tasksForDate);
        }
    }
}
