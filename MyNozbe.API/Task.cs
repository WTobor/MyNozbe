using System;

namespace MyNozbe.API
{
    public class Task
    {
        public Task(string name, bool isCompleted = false)
        {
            Name = name;
            IsCompleted = isCompleted;
        }

        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }
}
