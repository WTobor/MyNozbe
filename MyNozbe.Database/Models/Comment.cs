using System;
using System.ComponentModel.DataAnnotations;

namespace MyNozbe.Database.Models
{
    public class Comment
    {
        public Comment(int taskId, string content, DateTimeOffset creationDateTime)
        {
            TaskId = taskId;
            Content = content;
            CreationDateTime = creationDateTime;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTimeOffset CreationDateTime { get; set; }

        public int TaskId { get; set; }

        public virtual Task Task { get; set; }
    }
}