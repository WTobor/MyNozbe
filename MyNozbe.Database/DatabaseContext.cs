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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasMany(p => p.Tasks).WithOne(t => t.Project);
            base.OnModelCreating(modelBuilder);
        }
    }
}