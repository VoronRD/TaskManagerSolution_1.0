using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaskManager.Core.Enums;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;

namespace TaskManager.TasksModule.ViewModels
{
    public class TaskEditViewModel : BindableBase, INavigationAware
    {
        private readonly IDataService _dataService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private DateTime? _selectedDate;

        public TaskEditViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            GoBackCommand = new DelegateCommand(() =>
            {
                var parameters = new NavigationParameters();
                if (_selectedDate.HasValue)
                    parameters.Add("selectedDate", _selectedDate.Value);

                _regionManager.RequestNavigate("MainRegion", "TaskListView", parameters);
            });

            SaveCommand = new DelegateCommand(Save);

            RepeatIntervals = new List<RepeatInterval> { RepeatInterval.None, RepeatInterval.Daily, RepeatInterval.Weekly, RepeatInterval.Monthly };
            Statuses = new List<TasksStatus> { TasksStatus.InProgress, TasksStatus.Completed, TasksStatus.OnHold };
            Priorities = new List<Priority> { Priority.Low, Priority.Medium, Priority.High };

            AllTags = new ObservableCollection<Tags>(_dataService.LoadTags());

            _eventAggregator.GetEvent<TagsChangedEvent>().Subscribe(OnTagsChanged);
        }


        private void OnTagsChanged()
        {
            var updatedTags = _dataService.LoadTags();

            // Обновляем коллекцию AllTags без потери привязок
            AllTags.Clear();
            foreach (var tag in updatedTags)
                AllTags.Add(tag);

            // При необходимости обновить SelectedTags, если среди выбранных есть удалённые теги
            var selectedTagsIds = SelectedTags.Select(t => t.Id).ToHashSet();
            var filteredSelectedTags = AllTags.Where(t => selectedTagsIds.Contains(t.Id)).ToList();

            SelectedTags = new ObservableCollection<Tags>(filteredSelectedTags);
        }

        private Tasks _task;
        public Tasks Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }

        public ObservableCollection<Tags> AllTags { get; set; }
        private ObservableCollection<Tags> _selectedTags = new();
        public ObservableCollection<Tags> SelectedTags
        {
            get => _selectedTags;
            set
            {
                SetProperty(ref _selectedTags, value ?? new ObservableCollection<Tags>());
                Task.Tags = _selectedTags.ToList();
            }
        }

        public List<RepeatInterval> RepeatIntervals { get; }
        public List<TasksStatus> Statuses { get; }
        public List<Priority> Priorities { get; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand GoBackCommand { get; }

        private void Save()
        {
            // Обновляем теги в объекте Task из SelectedTags
            Task.Tags = SelectedTags?.ToList() ?? new List<Tags>();

            var tasks = _dataService.LoadTasks();

            // Добавляем повторяющиеся задачи, если есть
            if (Task.Repeat != RepeatInterval.None && Task.RepeatUntil.HasValue)
            {
                var repeatDate = Task.Deadline;
                while (true)
                {
                    repeatDate = GetNextDate(repeatDate, Task.Repeat);
                    if (repeatDate > Task.RepeatUntil.Value)
                        break;

                    var repeatedTask = new Tasks
                    {
                        Id = Guid.NewGuid(),
                        Title = Task.Title,
                        Description = Task.Description,
                        Priority = Task.Priority,
                        Status = Task.Status,
                        CreatedDate = DateTime.Now,
                        Deadline = repeatDate,
                        Repeat = RepeatInterval.None,
                        Tags = new List<Tags>(Task.Tags)
                    };
                    tasks.Add(repeatedTask);
                }
            }

            if (Task.Id == Guid.Empty)
                Task.Id = Guid.NewGuid();

            var existing = tasks.FirstOrDefault(t => t.Id == Task.Id);
            if (existing != null)
            {
                var index = tasks.IndexOf(existing);
                tasks[index] = Task;
            }
            else
            {
                tasks.Add(Task);
            }

            _dataService.SaveTasks(tasks);
            _eventAggregator.GetEvent<TaskChangedEvent>().Publish(Task);

            var parameters = new NavigationParameters();
            if (_selectedDate.HasValue)
                parameters.Add("selectedDate", _selectedDate.Value);

            _regionManager.RequestNavigate("MainRegion", "TaskListView", parameters);
        }




        private DateTime GetNextDate(DateTime current, RepeatInterval interval)
        {
            return interval switch
            {
                RepeatInterval.Daily => current.AddDays(1),
                RepeatInterval.Weekly => current.AddDays(7),
                RepeatInterval.Monthly => current.AddMonths(1),
                _ => current
            };
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("selectedDate", out DateTime selectedDate))
            {
                _selectedDate = selectedDate;
            }

            if (navigationContext.Parameters.TryGetValue("task", out Tasks task))
            {
                Task = task;
                SelectedTags = new ObservableCollection<Tags>(task.Tags ?? new List<Tags>());
            }
            else
            {
                Task = new Tasks
                {
                    CreatedDate = DateTime.Now,
                    Deadline = _selectedDate ?? DateTime.Now,
                    Status = TasksStatus.InProgress,
                    Priority = Priority.Medium,
                    Tags = new List<Tags>()
                };

                SelectedTags = new ObservableCollection<Tags>();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
