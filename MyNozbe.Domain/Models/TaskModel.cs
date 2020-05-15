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
        }

        public int Id { get; }

        public string Name { get; }

        public bool IsCompleted { get; private set; }

        public void MarkAsOpened()
        {
            IsCompleted = false;
        }

        public void MarkAsClosed()
        {
            IsCompleted = true;
        }
    }
}