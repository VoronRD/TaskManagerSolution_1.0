using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Models
{
    public class Tasks
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public TasksStatus Status { get; set; }
        public List<Tags> Tags { get; set; } = new();
        public Priority Priority { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime Deadline { get; set; } = DateTime.Now;
        public RepeatInterval Repeat { get; set; }
        public DateTime? RepeatUntil { get; set; }

        public bool IsDeadlineApproaching
        {
            get
            {
                return (Deadline - DateTime.Now).TotalDays <= 1 && Status != TasksStatus.Completed;
            }
        }

    }
}
