using TaskManager.Infrastructure.Services;

namespace TaskManager.Shell.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDataService _dataService;

        public MainWindowViewModel(IRegionManager regionManager, IDataService dataService)
        {
            _regionManager = regionManager;
            _dataService = dataService;
            NavigateCommand = new DelegateCommand<string>(Navigate);

            // Инициализация данных
            _dataService.LoadTasks();
            _dataService.LoadTags();
        }

        public DelegateCommand<string> NavigateCommand { get; }

        private void Navigate(string viewName)
        {
            _regionManager.RequestNavigate("MainRegion", viewName);
        }
    }
}