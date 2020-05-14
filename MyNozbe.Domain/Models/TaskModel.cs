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

        public int Id { get; }

        public string Name { get; }

        public bool IsCompleted { get; }
    }
}