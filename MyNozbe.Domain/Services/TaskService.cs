using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class TaskService
    {
        private readonly IDbOperations<TaskModel> _taskModelDbOperations;

        public TaskService(IDbOperations<TaskModel> taskModelDbOperations)
        {
            this._taskModelDbOperations = taskModelDbOperations;
        }

        public TaskModel AddTask(string name)
        {
            var taskModel = new TaskModel(name);
            taskModel = _taskModelDbOperations.Create(taskModel);

            return taskModel;
        }

        public TaskModel MarkTaskAsOpened(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return null;
            }

            taskModel.MarkAsOpened();
            _taskModelDbOperations.Update(taskModel);
            return taskModel;
        }

        public TaskModel MarkTaskAsClosed(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return null;
            }

            taskModel.MarkAsClosed();
            _taskModelDbOperations.Update(taskModel);
            return taskModel;
        }
    }
}