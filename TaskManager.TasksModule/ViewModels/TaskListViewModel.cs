using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;

namespace TaskManager.TasksModule.ViewModels
{
    public class TaskListViewModel : BindableBase, INavigationAware
    {
        private readonly IDataService _dataService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private DateTime? _selectedDate;

        public TaskListViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            // Инициализация коллекций
            AllTags = new ObservableCollection<Tags>(_dataService.LoadTags());
            SelectedTags = new ObservableCollection<Tags>();
            SortOptions = new List<string> { "Дата создания", "Дедлайн", "Приоритет", "Статус" };
            SelectedSort = SortOptions[0];
            SelectedStatus = "Все";

            // Команды
            CreateTaskCommand = new DelegateCommand(OpenCreateTask);
            EditTaskCommand = new DelegateCommand(OpenEditTask, CanEdit).ObservesProperty(() => SelectedTask);
            DeleteTaskCommand = new DelegateCommand(DeleteTask, CanEdit).ObservesProperty(() => SelectedTask);
            GoBackCommand = new DelegateCommand(() => _regionManager.RequestNavigate("MainRegion", "CalendarView"));
            ApplyFiltersCommand = new DelegateCommand(ApplyFilters);
            ResetFiltersCommand = new DelegateCommand(ResetFilters);

            // Подписка на события
            _eventAggregator.GetEvent<TaskChangedEvent>().Subscribe(OnTaskUpdated);
            _eventAggregator.GetEvent<TaskDeletedEvent>().Subscribe(OnTaskDeleted);

            // Подписка на событие изменения тегов
            _eventAggregator.GetEvent<TagsChangedEvent>().Subscribe(OnTagsChanged);
        }

        public ObservableCollection<Tasks> Tasks { get; private set; } = new();
        public ObservableCollection<Tags> AllTags { get; }
        public ObservableCollection<Tags> SelectedTags { get; set; }

        public List<string> SortOptions { get; }
        private string _selectedSort;
        public string SelectedSort
        {
            get => _selectedSort;
            set => SetProperty(ref _selectedSort, value);
        }

        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private Tasks _selectedTask;
        public Tasks SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public bool HasTasks => Tasks.Any();

        public DelegateCommand CreateTaskCommand { get; }
        public DelegateCommand EditTaskCommand { get; }
        public DelegateCommand DeleteTaskCommand { get; }
        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand ApplyFiltersCommand { get; }
        public DelegateCommand ResetFiltersCommand { get; }
        public DelegateCommand<Tasks> SelectTaskCommand => new DelegateCommand<Tasks>(task => SelectedTask = task);

        private List<Tasks> _allTasks = new();

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _allTasks = _dataService.LoadTasks();

            if (navigationContext.Parameters.TryGetValue("selectedDate", out DateTime selectedDate))
            {
                _selectedDate = selectedDate;
                _allTasks = _allTasks.Where(t => t.Deadline.Date == selectedDate.Date).ToList();
            }
            else
            {
                _selectedDate = null;
            }

            ResetFilters();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        private void ApplyFilters()
        {
            IEnumerable<Tasks> filtered = _allTasks;

            // Фильтрация по статусу
            if (SelectedStatus != null && SelectedStatus != "Все")
            {
                if (Enum.TryParse(SelectedStatus, out Core.Enums.TasksStatus status))
                {
                    filtered = filtered.Where(t => t.Status == status);
                }
            }

            // Фильтрация по тегам
            if (SelectedTags != null && SelectedTags.Count > 0)
            {
                filtered = filtered.Where(t => t.Tags != null && SelectedTags.All(st => t.Tags.Any(tt => tt.Id == st.Id)));
            }

            // Поиск по заголовку, описанию, тегам
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lowerSearch = SearchText.ToLowerInvariant();
                filtered = filtered.Where(t =>
                    (!string.IsNullOrEmpty(t.Title) && t.Title.ToLowerInvariant().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(t.Description) && t.Description.ToLowerInvariant().Contains(lowerSearch)) ||
                    (t.Tags != null && t.Tags.Any(tag => tag.Name.ToLowerInvariant().Contains(lowerSearch))));
            }

            // Сортировка
            filtered = SelectedSort switch
            {
                "Дата создания" => filtered.OrderBy(t => t.CreatedDate),
                "Дедлайн" => filtered.OrderBy(t => t.Deadline),
                "Приоритет" => filtered.OrderBy(t => t.Priority),
                "Статус" => filtered.OrderBy(t => t.Status),
                _ => filtered.OrderBy(t => t.CreatedDate)
            };

            Tasks = new ObservableCollection<Tasks>(filtered);
            RaisePropertyChanged(nameof(Tasks));
            RaisePropertyChanged(nameof(HasTasks));

            if (Tasks.Any())
                SelectedTask = Tasks.First();
            else
                SelectedTask = null;
        }

        private void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedStatus = "Все";
            SelectedTags.Clear();
            SelectedSort = SortOptions[0];
            Tasks = new ObservableCollection<Tasks>(_allTasks);
            RaisePropertyChanged(nameof(Tasks));
            RaisePropertyChanged(nameof(HasTasks));
            SelectedTask = Tasks.FirstOrDefault();
        }

        // Навигация и команды
        private void OpenCreateTask()
        {
            var parameters = new NavigationParameters();
            if (_selectedDate.HasValue)
                parameters.Add("selectedDate", _selectedDate.Value);
            _regionManager.RequestNavigate("MainRegion", "TaskEditView", parameters);
        }

        private void OpenEditTask()
        {
            if (SelectedTask == null)
                return;

            var parameters = new NavigationParameters { { "task", SelectedTask } };
            if (_selectedDate.HasValue)
                parameters.Add("selectedDate", _selectedDate.Value);
            _regionManager.RequestNavigate("MainRegion", "TaskEditView", parameters);
        }

        private void DeleteTask()
        {
            if (SelectedTask == null)
                return;

            var allTasks = _dataService.LoadTasks();
            var taskToRemove = allTasks.FirstOrDefault(t => t.Id == SelectedTask.Id);
            if (taskToRemove != null)
            {
                allTasks.Remove(taskToRemove);
                _dataService.SaveTasks(allTasks);
                _eventAggregator.GetEvent<TaskDeletedEvent>().Publish(taskToRemove);
            }

            Tasks.Remove(SelectedTask);
            RaisePropertyChanged(nameof(Tasks));
            RaisePropertyChanged(nameof(HasTasks));
        }

        private bool CanEdit() => SelectedTask != null;

        // Обновление при изменении задачи
        private void OnTaskUpdated(Tasks updatedTask)
        {
            var existing = Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (existing != null)
            {
                var index = Tasks.IndexOf(existing);
                Tasks[index] = updatedTask;
            }
            else
            {
                Tasks.Add(updatedTask);
            }

            RaisePropertyChanged(nameof(HasTasks));
        }

        private void OnTaskDeleted(Tasks deletedTask)
        {
            _allTasks = _dataService.LoadTasks();

            // При фильтрации по дате
            if (_selectedDate.HasValue)
            {
                _allTasks = _allTasks.Where(t => t.Deadline.Date == _selectedDate.Value.Date).ToList();
            }

            ResetFilters();
        }

        // Новое: обновление тегов при их изменении
        private void OnTagsChanged()
        {
            // Обновляем список тегов для фильтрации
            var tagsFromDataService = _dataService.LoadTags();

            // Очистка текущего списка
            AllTags.Clear();

            // Добавление актуальных тегов
            foreach (var tag in tagsFromDataService)
            {
                AllTags.Add(tag);
            }
        }
    }
}
