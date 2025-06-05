using TaskManager.Infrastructure.Services;

namespace TaskManager.SettingsModule.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISettingsService _settingsService;

        public DelegateCommand NavigateToTagsCommand { get; }
        public DelegateCommand ApplySettingsCommand { get; }
        public DelegateCommand GoBackCommand { get; }

        private int _notificationDays = 1;
        public int NotificationDays
        {
            get => _notificationDays;
            set => SetProperty(ref _notificationDays, value);
        }

        private bool _notificationsEnabled;
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => SetProperty(ref _notificationsEnabled, value);
        }

        public SettingsViewModel(IRegionManager regionManager, ISettingsService settingsService)
        {
            _regionManager = regionManager;
            _settingsService = settingsService;

            GoBackCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "CalendarView");
            });

            NotificationDays = _settingsService.GetNotificationDays();
            NotificationsEnabled = _settingsService.GetNotificationsEnabled();

            NavigateToTagsCommand = new DelegateCommand(OpenTagsModule);
            ApplySettingsCommand = new DelegateCommand(SaveSettings);
        }

        private void OpenTagsModule()
        {
            _regionManager.RequestNavigate("MainRegion", "TagListView");
        }

        private void SaveSettings()
        {
            _settingsService.SetNotificationDays(NotificationDays);
            _settingsService.SetNotificationsEnabled(NotificationsEnabled);
        }
    }
}
