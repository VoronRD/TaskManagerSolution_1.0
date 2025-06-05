using System.Windows.Media;

namespace TaskManager.TagsModule.Models
{
    public class SelectableColor : BindableBase
    {
        public Color Color { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
