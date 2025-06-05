using TaskManager.TagsModule.View;
using TaskManager.TagsModule.ViewModels;

namespace TaskManager.TagsModule
{
    public class TagsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RequestNavigate("MainRegion", "TagListView"); // Стартовая View
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TagListView, TagListViewModel>("TagListView");
            containerRegistry.RegisterForNavigation<TagEditView, TagEditViewModel>("TagEditView");
        }
    }
}
