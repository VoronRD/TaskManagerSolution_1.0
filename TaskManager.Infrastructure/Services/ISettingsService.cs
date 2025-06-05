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
