using Microsoft.Toolkit.Uwp.Notifications;
using TaskManager.Core.Enums;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ISettingsService _settingsService;

        public NotificationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }


        public void CheckUpcomingDeadlines(List<Tasks> tasks, int daysBeforeDeadline)
        {
            if (!_settingsService.GetNotificationsEnabled())
                return;

            var now = DateTime.Now.Date;

            var upcoming = tasks.Where(task =>
                task.Deadline.Date > now &&
                (task.Deadline.Date - now).TotalDays <= daysBeforeDeadline &&
                task.Status != TasksStatus.Completed
            );

            foreach (var task in upcoming)
            {
                new ToastContentBuilder()
                    .AddText("Скоро дедлайн по задаче")
                    .AddText($"{task.Title} — до {task.Deadline:dd.MM.yyyy}")
                    .Show();
            }
        }
    }
}
