using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System.Collections.Generic;
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

        public List<RepeatInterval> RepeatIntervals { get; }

    public TaskEditViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            RepeatIntervals = new List<RepeatInterval>
            {
                RepeatInterval.None,
                RepeatInterval.Daily,
                RepeatInterval.Weekly,
                RepeatInterval.Monthly
            };

            SaveCommand = new DelegateCommand(Save);

            Statuses = new List<TasksStatus> { TasksStatus.InProgress, TasksStatus.Completed, TasksStatus.OnHold };
            Priorities = new List<Priority> { Priority.Low, Priority.Medium, Priority.High };

            Task = new Tasks
            {
                CreatedDate = DateTime.Now,
                Status = TasksStatus.InProgress,
                Priority = Priority.Medium
            };
            _eventAggregator = eventAggregator;
        }

        private Tasks _task = new Tasks();
        public Tasks Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }

        public List<TasksStatus> Statuses { get; }
        public List<Priority> Priorities { get; }

        public DelegateCommand SaveCommand { get; }

        private void Save()
        {
            var tasks = _dataService.LoadTasks();

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
                        Repeat = RepeatInterval.None, // исключаем вложенные повторения
                        Tags = new List<Tags>(Task.Tags)
                    };
                    tasks.Add(repeatedTask);
                }
            }

            var existing = tasks.FirstOrDefault(t => t.Id == Task.Id);
            if (existing != null)
            {
                var index = tasks.IndexOf(existing);
                tasks[index] = Task;
            }
            else
            {
                Task.Id = Guid.NewGuid();
                tasks.Add(Task);
            }

            _dataService.SaveTasks(tasks);
            _eventAggregator.GetEvent<TaskChangedEvent>().Publish(Task);
            _regionManager.RequestNavigate("MainRegion", "TaskListView");
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
            if (navigationContext.Parameters.ContainsKey("task"))
            {
                Task = navigationContext.Parameters.GetValue<Tasks>("task");
            }
            else
            {
                Task = new Tasks
                {
                    CreatedDate = DateTime.Now,
                    Status = TasksStatus.InProgress,
                    Priority = Priority.Medium
                };
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}