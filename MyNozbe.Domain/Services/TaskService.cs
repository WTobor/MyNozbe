using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<OperationResult<int>> AddTaskAsync(string name)
        {
            var taskModel = new TaskModel(name);
            var result = await _taskModelValidator.ValidateAsync(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<int>(result);
            }

            var taskId = await _taskModelDbOperations.AddAsync(taskModel);
            return OperationResult<int>.Ok(taskId);
        }

        public async Task<OperationResult<TaskModel>> MarkTaskAsOpenedAsync(int taskId)
        {
            var taskModel = await _taskModelDbOperations.GetAsync(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.MarkAsOpened();
            await _taskModelDbOperations.UpdateAsync(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public async Task<OperationResult<TaskModel>> MarkTaskAsClosedAsync(int taskId)
        {
            var taskModel = await _taskModelDbOperations.GetAsync(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.MarkAsClosed();
            await _taskModelDbOperations.UpdateAsync(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public async Task<OperationResult<TaskModel>> RenameAsync(int taskId, string name)
        {
            var taskModel = await _taskModelDbOperations.GetAsync(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.Rename(name);

            var result = await _taskModelValidator.ValidateAsync(taskModel);
            if (!result.IsValid)
            {
                return GetValidationFailedOperationResult<TaskModel>(result);
            }

            await _taskModelDbOperations.UpdateAsync(taskModel);
            return OperationResult<TaskModel>.Ok();
        }

        public async Task<OperationResult<TaskModel>> AssignProjectAsync(int taskId, int projectId)
        {
            var taskModel = await _taskModelDbOperations.GetAsync(taskId);
            if (taskModel == null)
            {
                return OperationResult<TaskModel>.NotFound();
            }

            taskModel.AssignToProject(projectId);
            await _taskModelDbOperations.UpdateAsync(taskModel);
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