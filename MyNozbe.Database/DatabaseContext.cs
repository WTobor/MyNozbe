using Microsoft.EntityFrameworkCore;
using MyNozbe.Database.Models;

namespace MyNozbe.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Project> Projects { get; set; }
    }
}