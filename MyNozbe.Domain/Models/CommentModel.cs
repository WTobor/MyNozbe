namespace MyNozbe.Domain.Models
{
    public class CommentModel
    {
        public CommentModel(int taskId, string content)
        {
            TaskId = taskId;
            Content = content;
        }

        public CommentModel(int id, int taskId, string content)
        {
            Id = id;
            TaskId = taskId;
            Content = content;
        }

        public int Id { get; set; }

        public string Content { get; set; }

        public int TaskId { get; set; }
    }
}