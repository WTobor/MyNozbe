using System.Collections.Generic;

namespace MyNozbe.Domain.Models
{
    public class ProjectModel
    {
        public ProjectModel(string name)
        {
            Name = name;
        }

        public ProjectModel(int id, string name, IEnumerable<TaskModel> tasks)
        {
            Id = id;
            Name = name;
            TaskModels = tasks;
        }

        public int Id { get; }

        public string Name { get; }

        public IEnumerable<TaskModel> TaskModels { get; }
    }
}