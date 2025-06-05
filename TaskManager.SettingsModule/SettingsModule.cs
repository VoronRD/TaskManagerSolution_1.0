using TaskManager.SettingsModule.View;
using TaskManager.SettingsModule.ViewModels;

namespace TaskManager.SettingsModule
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RequestNavigate("MainRegion", "SettingsView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>("SettingsView");
        }
    }
}
