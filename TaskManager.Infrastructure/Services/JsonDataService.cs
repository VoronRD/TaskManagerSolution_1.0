using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Models;
using System.IO;
using Newtonsoft.Json;

namespace TaskManager.Infrastructure.Services
{
    public class JsonDataService : IDataService
    {
        private const string TasksFilePath = "tasks.json";
        private const string TagsFilePath = "tags.json";
        public List<Tags> LoadTags()
        {
            if(File.Exists(TagsFilePath))
            {
                string json = File.ReadAllText(TagsFilePath);
                return JsonConvert.DeserializeObject<List<Tags>>(json);
            }
            return new List<Tags>();
        }

        public List<Tasks> LoadTasks()
        {
            if (File.Exists(TasksFilePath))
            { 
                string json = File.ReadAllText(TasksFilePath);
                return JsonConvert.DeserializeObject<List<Tasks>>(json);
            }
            return new List<Tasks>();
        }

        public void SaveTags(List<Tags> tags)
        {
            string json = JsonConvert.SerializeObject(tags, Formatting.Indented);
            File.WriteAllText(TagsFilePath, json);
        }

        public void SaveTasks(List<Tasks> tasks)
        {
            string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(TasksFilePath, json);
        }
    }
}
