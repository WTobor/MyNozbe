using System.ComponentModel.DataAnnotations;

namespace MyNozbe.Database.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
