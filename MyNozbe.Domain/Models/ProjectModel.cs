using System.Collections.Generic;

namespace MyNozbe.Domain.Models
{
    public class ProjectModel
    {
        public ProjectModel(string name)
        {
            Name = name;
        }

        public ProjectModel(int id, string name, List<TaskModel> tasks)
        {
            Id = id;
            Name = name;
            TaskModels = tasks;
        }

        public int Id { get; }

        public string Name { get; private set; }

        public ICollection<TaskModel> TaskModels { get; private set; }

        public void Rename(string name)
        {
            Name = name;
        }
    }
}