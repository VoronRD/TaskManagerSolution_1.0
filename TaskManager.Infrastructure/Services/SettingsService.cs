using TaskManager.Infrastructure.Properties;

namespace TaskManager.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        public int GetNotificationDays()
        {
            return Settings.Default.NotificationDays;
        }

        public void SetNotificationDays(int days)
        {
            Settings.Default.NotificationDays = days;
            Settings.Default.Save();
        }

        public bool GetNotificationsEnabled()
        {
            return Settings.Default.NotificationsEnabled;
        }

        public void SetNotificationsEnabled(bool enabled)
        {
            Settings.Default.NotificationsEnabled = enabled;
            Settings.Default.Save();
        }
    }
}
