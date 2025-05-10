using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Events;
using TaskManager.Infrastructure.Services;

namespace TaskManager.TagsModule.ViewModels
{
    public class TagEditViewModel : BindableBase, INavigationAware
    {
        private readonly IDataService _dataService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private string _newTagName;
        private Color? _selectedColor;
        private Tags _editingTag;

        public string NewTagName
        {
            get => _newTagName;
            set => SetProperty(ref _newTagName, value);
        }

        public Color? SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public List<Color> AvailableColors { get; } = new()
        {
            Colors.Red, Colors.Green, Colors.Blue,
            Colors.Yellow, Colors.Orange, Colors.Purple,
            Colors.Pink, Colors.Brown, Colors.Cyan
        };

        public DelegateCommand CreateTagCommand { get; }
        public DelegateCommand<Color?> SelectColorCommand { get; }
        public DelegateCommand GoBackCommand { get; }

        public TagEditViewModel(IDataService dataService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            GoBackCommand = new DelegateCommand(() =>
            {
                regionManager.RequestNavigate("MainRegion", "TagListView");
            });

            CreateTagCommand = new DelegateCommand(CreateTag);
            SelectColorCommand = new DelegateCommand<Color?>(color => SelectedColor = color);
        }

        private void CreateTag()
        {
            if (string.IsNullOrWhiteSpace(NewTagName))
                return;

            var tags = _dataService.LoadTags();

            if (_editingTag != null)
            {
                var existing = tags.FirstOrDefault(t => t.Id == _editingTag.Id);
                if (existing != null)
                {
                    existing.Name = NewTagName;
                    existing.Color = SelectedColor;
                }
            }
            else
            {
                tags.Add(new Tags
                {
                    Id = Guid.NewGuid(),
                    Name = NewTagName,
                    Color = SelectedColor
                });
            }

            _dataService.SaveTags(tags);
            _eventAggregator.GetEvent<TagsChangedEvent>().Publish();
            _regionManager.RequestNavigate("MainRegion", "TagListView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("tag", out Tags tag))
            {
                _editingTag = tag;
                NewTagName = tag.Name;
                SelectedColor = tag.Color;
            }
            else
            {
                _editingTag = null;
                NewTagName = string.Empty;
                SelectedColor = Colors.LightGray;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
