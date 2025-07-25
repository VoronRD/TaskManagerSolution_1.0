﻿using TaskManager.TasksModule.ViewModels;
using TaskManager.TasksModule.Views;

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
            containerRegistry.RegisterForNavigation<TaskListView, TaskListViewModel>("TaskListView");
            containerRegistry.RegisterForNavigation<TaskEditView, TaskEditViewModel>("TaskEditView");
        }
    }
}