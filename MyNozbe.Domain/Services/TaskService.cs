using MyNozbe.Database;
using MyNozbe.Database.Models;

namespace MyNozbe.Domain.Services
{
    public class TaskService
    {
        private readonly DatabaseContext _databaseContext;

        public TaskService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task MarkTaskAsOpened(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return null;
            }

            task.IsCompleted = false;
            _databaseContext.SaveChanges();
            return task;
        }

        public Task MarkTaskAsClosed(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return null;
            }

            task.IsCompleted = true;
            _databaseContext.SaveChanges();
            return task;
        }
    }
}