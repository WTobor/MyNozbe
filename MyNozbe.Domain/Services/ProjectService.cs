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

        public OperationResult<int> AddProject(string name)
        {
            var projectModel = new ProjectModel(name);

            var projectId = _projectModelDbOperations.Add(projectModel);
            return OperationResult<int>.Ok(projectId);
        }
    }
}