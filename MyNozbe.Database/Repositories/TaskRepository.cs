﻿using System;
using System.Threading.Tasks;
using MyNozbe.Database.Mappers;
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

        public async Task<int> AddAsync(TaskModel model)
        {
            var task = new Models.Task(model.Name, DateTimeOffset.Now, model.IsCompleted);
            await _databaseContext.Tasks.AddAsync(task);
            await _databaseContext.SaveChangesAsync();
            return task.Id;
        }

        public async Task<TaskModel> GetAsync(int taskId)
        {
            var task = await _databaseContext.Tasks.FindAsync(taskId);
            return TaskMapper.MapTaskToTaskModel(task);
        }

        public async Task UpdateAsync(TaskModel model)
        {
            var task = await _databaseContext.Tasks.FindAsync(model.Id);
            task.Name = model.Name;
            task.IsCompleted = model.IsCompleted;
            task.ProjectId = model.ProjectId;
            await _databaseContext.SaveChangesAsync();
        }
    }
}