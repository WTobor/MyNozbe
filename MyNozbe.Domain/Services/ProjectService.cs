using System.Threading.Tasks;
using FluentValidation;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class ProjectService
    {
        private readonly IDbOperations<ProjectModel> _projectModelDbOperations;
        private readonly IValidator<ProjectModel> _projectModelValidator;

        public ProjectService(IDbOperations<ProjectModel> projectModelDbOperations,
            IValidator<ProjectModel> projectModelValidator)
        {
            _projectModelDbOperations = projectModelDbOperations;
            _projectModelValidator = projectModelValidator;
        }

        public async Task<OperationResult<int>> AddProjectAsync(string name)
        {
            var projectModel = new ProjectModel(name);
            var validationResult = await _projectModelValidator.ValidateAsync(projectModel);
            if (!validationResult.IsValid)
            {
                return ValidationHelper.GetValidationFailedOperationResult<int>(validationResult);
            }

            var projectId = await _projectModelDbOperations.AddAsync(projectModel);
            return OperationResult<int>.Ok(projectId);
        }

        public async Task<OperationResult<ProjectModel>> RenameAsync(int projectId, string name)
        {
            var projectModel = await _projectModelDbOperations.GetAsync(projectId);
            if (projectModel == null)
            {
                return OperationResult<ProjectModel>.NotFound();
            }

            projectModel.Rename(name);
            var validationResult = await _projectModelValidator.ValidateAsync(projectModel);
            if (!validationResult.IsValid)
            {
                return ValidationHelper.GetValidationFailedOperationResult<ProjectModel>(validationResult);
            }

            await _projectModelDbOperations.UpdateAsync(projectModel);
            return OperationResult<ProjectModel>.Ok();
        }
    }
}