using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class TaskService
    {
        private readonly IDbOperations<TaskModel> _taskModelDbOperations;
        private readonly IValidator<TaskModel> _taskModelValidator;

        public TaskService(IDbOperations<TaskModel> taskModelDbOperations, IValidator<TaskModel> taskModelValidator)
        {
            _taskModelDbOperations = taskModelDbOperations;
            _taskModelValidator = taskModelValidator;
        }

        public OperationResult<int> AddTask(string name)
        {
            var taskModel = new TaskModel(name);
            var result = _taskModelValidator.Validate(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<int>(result);
            }

            var taskId = _taskModelDbOperations.Add(taskModel);
            return OperationResult<int>.Ok(taskId);
        }

        public OperationResult<TaskModel> MarkTaskAsOpened(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.MarkAsOpened();
            _taskModelDbOperations.Update(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public OperationResult<TaskModel> MarkTaskAsClosed(int taskId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.MarkAsClosed();
            _taskModelDbOperations.Update(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public OperationResult<TaskModel> Rename(int taskId, string name)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.Rename(name);

            var result = _taskModelValidator.Validate(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<TaskModel>(result);
            }

            _taskModelDbOperations.Update(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public OperationResult<TaskModel> AssignProject(int taskId, int projectId)
        {
            var taskModel = _taskModelDbOperations.Get(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }
            taskModel.AssignToProject(projectId);
            _taskModelDbOperations.Update(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        private static OperationResult<T> GetValidationFailedOperationResult<T>(ValidationResult result)
        {
            var validationErrors = GetValidationErrorMessage(result.Errors);
            return OperationResult<T>.ValidationFailed(validationErrors);
        }

        private static string GetValidationErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            return string.Join(";", errors.Select(x => x.ErrorMessage));
        }
    }
}