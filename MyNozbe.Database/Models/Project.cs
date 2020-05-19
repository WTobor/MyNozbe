using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNozbe.Database.Models
{
    public class Project
    {
        public Project(string name, DateTimeOffset creationDateTime)
        {
            Name = name;
            CreationDateTime = creationDateTime;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTimeOffset CreationDateTime { get; set; }

        public IEnumerable<Task> Tasks { get; set; }
    }
}