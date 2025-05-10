using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;

namespace TaskManager.TagsModule.ViewModels
{
    public class TagListViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private Tags _selectedTag;

        public ObservableCollection<Tags> Tags { get; }

        public Tags SelectedTag
        {
            get => _selectedTag;
            set => SetProperty(ref _selectedTag, value);
        }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand DeleteTagCommand { get; }
        public DelegateCommand<Tags> SelectTagCommand { get; }
        public DelegateCommand GoBackCommand { get; }

        public TagListViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            GoBackCommand = new DelegateCommand(() =>
            {
                regionManager.RequestNavigate("MainRegion", "SettingsView");
            });

            Tags = new ObservableCollection<Tags>(_dataService.LoadTags());

            AddTagCommand = new DelegateCommand(OnAddTag);
            DeleteTagCommand = new DelegateCommand(OnDeleteTag, () => SelectedTag != null)
                .ObservesProperty(() => SelectedTag);
            SelectTagCommand = new DelegateCommand<Tags>(OnSelectTag);

            _eventAggregator.GetEvent<TagsChangedEvent>().Subscribe(RefreshTags);
        }

        private void OnAddTag()
        {
            _regionManager.RequestNavigate("MainRegion", "TagEditView");
        }

        private void OnDeleteTag()
        {
            if (SelectedTag != null)
            {
                Tags.Remove(SelectedTag);
                _dataService.SaveTags(Tags.ToList());
                _eventAggregator.GetEvent<TagsChangedEvent>().Publish();
            }
        }

        private void OnSelectTag(Tags tag)
        {
            var parameters = new NavigationParameters
            {
                { "tag", tag }
            };
            _regionManager.RequestNavigate("MainRegion", "TagEditView", parameters);
        }

        private void RefreshTags()
        {
            Tags.Clear();
            foreach (var tag in _dataService.LoadTags())
                Tags.Add(tag);
        }
    }
}
