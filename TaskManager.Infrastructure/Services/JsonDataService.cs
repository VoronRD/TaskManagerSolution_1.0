using TaskManager.Core.Models;
using System.IO;
using Newtonsoft.Json;

namespace TaskManager.Infrastructure.Services
{
    public class JsonDataService : IDataService
    {
        private const string TasksFilePath = "tasks.json";
        private const string TagsFilePath = "tags.json";

        public List<Tasks> LoadTasks()
        {
            if (File.Exists(TasksFilePath))
            {
                string json = File.ReadAllText(TasksFilePath);
                return JsonConvert.DeserializeObject<List<Tasks>>(json) ?? new List<Tasks>();
            }
            return new List<Tasks>();
        }

        public List<Tags> LoadTags()
        {
            if (File.Exists(TagsFilePath))
            {
                string json = File.ReadAllText(TagsFilePath);
                return JsonConvert.DeserializeObject<List<Tags>>(json) ?? new List<Tags>();
            }
            return new List<Tags>();
        }


        public void SaveTags(List<Tags> tags)
        {
            string json = JsonConvert.SerializeObject(tags, Formatting.Indented);
            File.WriteAllText(TagsFilePath, json);
        }

        public void SaveTasks(List<Tasks> tasks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(TasksFilePath, json);
            }
            catch (Exception ex)
            {
                // логирование или уведомление пользователя
                Console.WriteLine("Ошибка сохранения задач: " + ex.Message);
            }
        }


        // Новый метод для сохранения или обновления задачи
        public void SaveOrUpdateTask(Tasks task)
        {
            var tasks = LoadTasks();
            var existing = tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                var index = tasks.IndexOf(existing);
                tasks[index] = task; // Обновляем существующую задачу
            }
            else
            {
                tasks.Add(task); // Добавляем новую задачу
            }

            SaveTasks(tasks); // Сохраняем обновленный список задач
        }
    }


}
