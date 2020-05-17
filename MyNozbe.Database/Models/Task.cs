using System;
using System.ComponentModel.DataAnnotations;

namespace MyNozbe.Database.Models
{
    public class Task
    {
        public Task(string name, DateTimeOffset creationDateTime, bool isCompleted)
        {
            Name = name;
            CreationDateTime = creationDateTime;
            IsCompleted = isCompleted;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [Required]
        public DateTimeOffset CreationDateTime { get; set; }
    }
}