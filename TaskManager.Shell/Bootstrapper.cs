using System.Windows;
using TaskManager.Calendar.View;
using TaskManager.Infrastructure.Services;
using TaskManager.Shell.ViewModels;
using TaskManager.Shell.Views;
using TaskManager.TasksModule.ViewModels;
using TaskManager.TasksModule.Views;
using TaskManager.Calendar.View;
using TaskManager.Calendar.ViewModels;

namespace TaskManager.Shell
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Регистрация сервисов
            containerRegistry.RegisterSingleton<IDataService, JsonDataService>();
            // Регистрация MainWindow и его ViewModel
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>("MainWindow");
            // Регистрация представлений и ViewModel
            containerRegistry.RegisterForNavigation<CalendarView, CalendarViewModel>("CalendarView");
            containerRegistry.RegisterForNavigation<TaskEditView, TaskEditViewModel>("TaskEditView");
            containerRegistry.RegisterForNavigation<TaskListView, TaskListViewModel>("TaskListView");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            // Регистрация модулей
            moduleCatalog.AddModule<TaskManager.Calendar.CalendarModule>();
            moduleCatalog.AddModule<TaskManager.TasksModule.TasksModule>();
            //moduleCatalog.AddModule<TagsModule>();
            //moduleCatalog.AddModule<SettingsModule>();
        }
    }
}