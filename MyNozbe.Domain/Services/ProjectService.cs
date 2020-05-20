using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class ProjectService
    {
        private readonly IDbOperations<ProjectModel> _projectModelDbOperations;
        private readonly IDbOperations<TaskModel> _taskModelDbOperations;

        public ProjectService(IDbOperations<ProjectModel> projectModelDbOperations,
            IDbOperations<TaskModel> taskModelDbOperations)
        {
            _projectModelDbOperations = projectModelDbOperations;
            _taskModelDbOperations = taskModelDbOperations;
        }

        public OperationResult<int> AddProject(string name)
        {
            var projectModel = new ProjectModel(name);

            var projectId = _projectModelDbOperations.Add(projectModel);
            return OperationResult<int>.Ok(projectId);
        }

        public OperationResult<ProjectModel> Rename(int projectId, string name)
        {
            var project = _projectModelDbOperations.Get(projectId);
            if (project == null)
            {
                return OperationResult<ProjectModel>.NotFound();
            }

            project.Rename(name);
            _projectModelDbOperations.Update(project);
            return OperationResult<ProjectModel>.Ok();
        }

        public OperationResult<ProjectModel> AssignTask(int projectId, int taskId)
        {
            var task = _taskModelDbOperations.Get(taskId);
            if (task == null)
            {
                return OperationResult<ProjectModel>.NotFound();
            }

            task.AssignToProject(projectId);
            _taskModelDbOperations.Update(task);
            return OperationResult<ProjectModel>.Ok();
        }
    }
}