using System;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Database.Repositories
{
    public class TaskRepository : IDbOperations<TaskModel>
    {
        private readonly DatabaseContext _databaseContext;

        public TaskRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public TaskModel Create(TaskModel model)
        {
            var task = new Task(model.Name, DateTimeOffset.Now);
            _databaseContext.Tasks.Add(task);
            _databaseContext.SaveChanges();
            var result = MapTaskToTaskModel(task);
            return result;
        }

        public TaskModel Get(int taskId)
        {
            var task = _databaseContext.Tasks.Find(taskId);
            if (task == null)
            {
                return null;
            }

            return new TaskModel(task.Id, task.Name, task.IsCompleted);
        }

        public void Update(TaskModel model)
        {
            var task = _databaseContext.Tasks.Find(model.Id);
            task.Name = model.Name;
            task.IsCompleted = model.IsCompleted;
            _databaseContext.SaveChanges();
        }

        private static TaskModel MapTaskToTaskModel(Task task)
        {
            return new TaskModel(task.Id, task.Name, task.IsCompleted);
        }
    }
}