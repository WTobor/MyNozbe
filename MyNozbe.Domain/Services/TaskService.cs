using MyNozbe.Database;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class TaskService
    {
        private readonly DatabaseContext _databaseContext;

        public TaskService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public TaskModel MarkTaskAsOpened(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return null;
            }

            task.IsCompleted = false;
            _databaseContext.SaveChanges();
            return new TaskModel(task.Id, task.Name, task.IsCompleted);
        }

        public TaskModel MarkTaskAsClosed(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return null;
            }

            task.IsCompleted = true;
            _databaseContext.SaveChanges();
            return new TaskModel(task.Id, task.Name, task.IsCompleted);
        }
    }
}