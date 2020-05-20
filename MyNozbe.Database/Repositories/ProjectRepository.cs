using System;
using System.Linq;
using MyNozbe.Database.Mappers;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Database.Repositories
{
    public class ProjectRepository : IDbOperations<ProjectModel>
    {
        private readonly DatabaseContext _databaseContext;

        public ProjectRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int Add(ProjectModel model)
        {
            var project = new Project(model.Name, DateTimeOffset.Now);
            _databaseContext.Projects.Add(project);
            _databaseContext.SaveChanges();
            return project.Id;
        }

        public ProjectModel Get(int projectId)
        {
            var project = _databaseContext.Projects.Find(projectId);
            return MapProjectToProjectModel(project);
        }

        public void Update(ProjectModel model)
        {
            var project = _databaseContext.Projects.Find(model.Id);
            project.Name = model.Name;
            project.Tasks = model.TaskModels?.Select(x => _databaseContext.Tasks.Find(x.Id)).ToList();

            _databaseContext.SaveChanges();
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
    }
}