
using Microsoft.EntityFrameworkCore;
using MyNozbe.Database.Models;

namespace MyNozbe.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }
    }
}
