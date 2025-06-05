using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Services
{
    public interface IDataService
    {
        List<Tasks> LoadTasks();
        void SaveTasks(List<Tasks> tasks);

        List<Tags> LoadTags();
        void SaveTags(List<Tags> tags);

        public void SaveOrUpdateTask(Tasks task);
    }
}
