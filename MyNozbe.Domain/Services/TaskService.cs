using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Validators;

namespace MyNozbe.Domain.Services
{
    public class TaskService
    {
        private readonly IDbOperations<TaskModel> _taskModelDbOperations;

        public TaskService(IDbOperations<TaskModel> taskModelDbOperations)
        {
            _taskModelDbOperations = taskModelDbOperations;
        }

        public OperationResult<int> AddTask(string name)
        {
            var taskModel = new TaskModel(name);
            var validator = new TaskModelValidator();
            var result = validator.Validate(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<int>(result);
            }

            var taskId = _taskModelDbOperations.Add(taskModel);
            return new OperationResult<int>(taskId);
        }

        public OperationResult<TaskModel> MarkTaskAsOpened(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return new OperationResult<TaskModel>(OperationResultStatus.NotFound);
            }

            taskModel.MarkAsOpened();
            _taskModelDbOperations.Update(taskModel);
            return new OperationResult<TaskModel>(taskModel);
        }

        public OperationResult<TaskModel> MarkTaskAsClosed(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return new OperationResult<TaskModel>(OperationResultStatus.NotFound);
            }

            taskModel.MarkAsClosed();
            _taskModelDbOperations.Update(taskModel);
            return new OperationResult<TaskModel>(taskModel);
        }

        public OperationResult<TaskModel> Rename(int taskId, string name)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return new OperationResult<TaskModel>(OperationResultStatus.NotFound);
            }

            taskModel.Rename(name);

            var validator = new TaskModelValidator();
            var result = validator.Validate(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<TaskModel>(result);
            }

            _taskModelDbOperations.Update(taskModel);
            return new OperationResult<TaskModel>(OperationResultStatus.Ok);
        }

        private OperationResult<T> GetValidationFailedOperationResult<T>(ValidationResult result)
        {
            var validationErrors = GetValidationErrorMessage(result.Errors);
            return new OperationResult<T>(validationErrors, OperationResultStatus.ValidationFailed);
        }

        private string GetValidationErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            return string.Join(";", errors.Select(x => x.ErrorMessage));
        }
    }
}