using System.Collections;
using System.Collections.Generic;

namespace MyNozbe.Domain.Models
{
    public class TaskModel
    {
        public TaskModel(int id, string name, bool isCompleted)
        {
            Id = id;
            Name = name;
            IsCompleted = isCompleted;
        }

        public TaskModel(string name)
        {
            Name = name;
            IsCompleted = false;
        }

        public int Id { get; }

        public string Name { get; private set; }

        public bool IsCompleted { get; private set; }

        public int? ProjectId { get; private set; }

        public ICollection<string> Comments { get; private set; }

        public void MarkAsOpened()
        {
            IsCompleted = false;
        }

        public void MarkAsClosed()
        {
            IsCompleted = true;
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void AssignToProject(int projectId)
        {
            ProjectId = projectId;
        }
    }
}