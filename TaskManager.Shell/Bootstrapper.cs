using System.Windows;
using TaskManager.Calendar.View;
using TaskManager.Infrastructure.Services;
using TaskManager.Shell.ViewModels;
using TaskManager.Shell.Views;
using TaskManager.TasksModule.ViewModels;
using TaskManager.TasksModule.Views;
using TaskManager.Calendar.ViewModels;
using TaskManager.TagsModule.ViewModels;
using TaskManager.TagsModule.View;
using TaskManager.SettingsModule.View;
using TaskManager.SettingsModule.ViewModels;

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
            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            // Регистрация MainWindow и его ViewModel
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>("MainWindow");

            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>("SettingsView");

            // Регистрация представлений и ViewModel
            containerRegistry.RegisterForNavigation<CalendarView, CalendarViewModel>("CalendarView");
            containerRegistry.RegisterForNavigation<TaskEditView, TaskEditViewModel>("TaskEditView");
            containerRegistry.RegisterForNavigation<TaskListView, TaskListViewModel>("TaskListView");
            containerRegistry.RegisterForNavigation<TagListView, TagListViewModel>("TagListView");
            containerRegistry.RegisterForNavigation<TagEditView, TagEditViewModel>("TagEditView");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            // Регистрация модулей
            moduleCatalog.AddModule<TaskManager.SettingsModule.SettingsModule>();
            moduleCatalog.AddModule<TaskManager.Calendar.CalendarModule>();
            moduleCatalog.AddModule<TaskManager.TasksModule.TasksModule>();
            moduleCatalog.AddModule<TaskManager.TagsModule.TagsModule>();
        }
    }
}