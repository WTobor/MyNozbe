using System;
using MyNozbe.Database.Mappers;
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

        public int Add(TaskModel model)
        {
            var task = new Task(model.Name, DateTimeOffset.Now, model.IsCompleted);
            _databaseContext.Tasks.Add(task);
            _databaseContext.SaveChanges();
            return task.Id;
        }

        public TaskModel Get(int taskId)
        {
            var task = _databaseContext.Tasks.Find(taskId);
            return TaskMapper.MapTaskToTaskModel(task);
        }

        public void Update(TaskModel model)
        {
            var task = _databaseContext.Tasks.Find(model.Id);
            task.Name = model.Name;
            task.IsCompleted = model.IsCompleted;
            task.ProjectId = model.ProjectId;
            _databaseContext.SaveChanges();
        }
    }
}