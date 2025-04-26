using Prism.Mvvm;
using Prism.Events;
using System.Collections.ObjectModel;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;
using System.Linq;

namespace TaskManager.TasksModule.ViewModels
{
    public class TaskListViewModel : BindableBase, INavigationAware
    {
        private readonly IDataService _dataService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public TaskListViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            Tasks = new ObservableCollection<Tasks>(_dataService.LoadTasks());

            CreateTaskCommand = new DelegateCommand(OpenCreateTask);
            EditTaskCommand = new DelegateCommand(OpenEditTask, CanEdit).ObservesProperty(() => SelectedTask);
            DeleteTaskCommand = new DelegateCommand(DeleteTask, CanEdit).ObservesProperty(() => SelectedTask);

            _eventAggregator.GetEvent<TaskChangedEvent>().Subscribe(OnTaskUpdated);
        }

        public DelegateCommand CreateTaskCommand { get; }
        public DelegateCommand EditTaskCommand { get; }
        public DelegateCommand DeleteTaskCommand { get; }

        private Tasks _selectedTask;
        public Tasks SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        private ObservableCollection<Tasks> _tasks;
        public ObservableCollection<Tasks> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        private void OpenCreateTask()
        {
            _regionManager.RequestNavigate("MainRegion", "TaskEditView");
        }

        private void OpenEditTask()
        {
            var parameters = new NavigationParameters
            {
                { "task", SelectedTask }
            };
            _regionManager.RequestNavigate("MainRegion", "TaskEditView", parameters);
        }

        private void DeleteTask()
        {
            Tasks.Remove(SelectedTask);
            _dataService.SaveTasks(Tasks.ToList());
            RaisePropertyChanged(nameof(Tasks));
        }

        private bool CanEdit() => SelectedTask != null;

        public DelegateCommand<Tasks> SelectTaskCommand => new DelegateCommand<Tasks>(task =>
        {
            SelectedTask = task;
        });

        private void OnTaskUpdated(Tasks updatedTask)
        {
            var existingTask = Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (existingTask != null)
            {
                var index = Tasks.IndexOf(existingTask);
                Tasks[index] = updatedTask;
            }
            else
            {
                Tasks.Add(updatedTask);
            }

            _dataService.SaveTasks(Tasks.ToList());
            RaisePropertyChanged(nameof(Tasks));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("selectedDate", out DateTime selectedDate))
            {
                Tasks = new ObservableCollection<Tasks>(
                    _dataService.LoadTasks()
                        .Where(t => t.Deadline.Date == selectedDate.Date)
                );
            }
            else
            {
                Tasks = new ObservableCollection<Tasks>(_dataService.LoadTasks());
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}