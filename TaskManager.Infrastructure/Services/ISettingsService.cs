using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Infrastructure.Services
{
    public interface ISettingsService
    {
        int GetNotificationDays();
        void SetNotificationDays(int days);
        bool GetNotificationsEnabled();
        void SetNotificationsEnabled(bool enabled);
    }
}
