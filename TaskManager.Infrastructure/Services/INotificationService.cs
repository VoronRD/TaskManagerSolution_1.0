using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Services
{
    public interface INotificationService
    {
        void CheckUpcomingDeadlines(List<Tasks> tasks, int daysBeforeDeadline);
    }

}
