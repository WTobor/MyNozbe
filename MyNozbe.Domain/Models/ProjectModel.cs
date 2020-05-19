using System.Collections.Generic;

namespace MyNozbe.Domain.Models
{
    public class ProjectModel
    {
        public ProjectModel(string name)
        {
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        public IEnumerable<TaskModel> TaskModels { get; private set; }
    }
}