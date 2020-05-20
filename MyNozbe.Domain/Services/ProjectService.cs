using System.Threading.Tasks;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class ProjectService
    {
        private readonly IDbOperations<ProjectModel> _projectModelDbOperations;

        public ProjectService(IDbOperations<ProjectModel> projectModelDbOperations)
        {
            _projectModelDbOperations = projectModelDbOperations;
        }

        public async Task<OperationResult<int>> AddProjectAsync(string name)
        {
            var projectModel = new ProjectModel(name);

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
            await _projectModelDbOperations.UpdateAsync(projectModel);
            return OperationResult<ProjectModel>.Ok();
        }
    }
}