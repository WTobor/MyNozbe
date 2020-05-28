using System;
using System.Linq;
using System.Threading.Tasks;
using MyNozbe.Database.Mappers;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace MyNozbe.Database.Repositories
{
    public class ProjectRepository : IDbOperations<ProjectModel>
    {
        private readonly DatabaseContext _databaseContext;

        public ProjectRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> AddAsync(ProjectModel model)
        {
            var project = new Project(model.Name, DateTimeOffset.Now);
            await _databaseContext.Projects.AddAsync(project);
            await _databaseContext.SaveChangesAsync();
            return project.Id;
        }

        public async Task<ProjectModel> GetAsync(int projectId)
        {
            var project = await _databaseContext.Projects.FindAsync(projectId);
            return MapProjectToProjectModel(project);
        }

        public async Task UpdateAsync(ProjectModel model)
        {
            var project = await _databaseContext.Projects.FindAsync(model.Id);
            project.Name = model.Name;

            await _databaseContext.SaveChangesAsync();
        }

        private static ProjectModel MapProjectToProjectModel(Project project)
        {
            if (project is null)
            {
                return null;
            }

            var tasks = project.Tasks?.Select(TaskMapper.MapTaskToTaskModel).ToList();

            return new ProjectModel(project.Id, project.Name, tasks);
        }

        public async Task DeleteAsync(ProjectModel model)
        {
            var project = await _databaseContext.Projects.FindAsync(model.Id);
            _databaseContext.Projects.Remove(project);
            await _databaseContext.SaveChangesAsync();
        }
    }
}