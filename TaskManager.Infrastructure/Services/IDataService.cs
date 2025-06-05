using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Services
{
    public interface IDataService
    {
        List<Tasks> LoadTasks();
        void SaveTasks(List<Tasks> tasks);

        List<Tags> LoadTags();
        void SaveTags(List<Tags> tags);
    }
}
