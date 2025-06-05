using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Events
{
    public class TaskDeletedEvent : PubSubEvent<Tasks>
    {
    }
}
