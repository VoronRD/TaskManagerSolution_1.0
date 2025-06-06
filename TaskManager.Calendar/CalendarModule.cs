﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Calendar.View;
using TaskManager.Calendar.ViewModels;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Calendar
{
    public class CalendarModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("MainRegion", "CalendarView");

            // Предзагрузка данных
            var dataService = containerProvider.Resolve<IDataService>();
            dataService.LoadTasks(); // Инициализация данных
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<CalendarView, CalendarViewModel>("CalendarView");
        }
    }
}
