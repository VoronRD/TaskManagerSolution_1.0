using System.Windows.Media;

namespace TaskManager.Core.Models
{
    public class Tags
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Color? Color { get; set; }
    }
}
