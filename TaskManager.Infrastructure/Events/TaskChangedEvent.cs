﻿using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Events
{
    public class TaskChangedEvent : PubSubEvent<Tasks>
    {
    }
}
