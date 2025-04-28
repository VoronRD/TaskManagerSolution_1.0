using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Events
{
    public class TaskDeletedEvent : PubSubEvent<Tasks>
    {
    }
}
