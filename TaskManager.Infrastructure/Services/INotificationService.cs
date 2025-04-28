using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Services
{
    public interface INotificationService
    {
        void CheckUpcomingDeadlines(List<Tasks> tasks, int daysBeforeDeadline);
    }
}
