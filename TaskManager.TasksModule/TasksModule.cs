using TaskManager.TasksModule.ViewModels;
using TaskManager.TasksModule.Views;
using TaskManager.Infrastructure.Services;

namespace TaskManager.TasksModule
{
    public class TasksModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RequestNavigate("MainRegion", "TaskListView"); // Стартовая View
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Регистрация View для навигации
            containerRegistry.RegisterForNavigation<TaskListView, TaskListViewModel>("TaskListView");
            containerRegistry.RegisterForNavigation<TaskEditView, TaskEditViewModel>("TaskEditView");
        }
    }
}