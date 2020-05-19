using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNozbe.Database;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;

namespace MyNozbe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;

        public ProjectController(ILogger<ProjectController> logger, DatabaseContext databaseContext,
            ProjectService projectService)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _projectService = projectService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Project>> GetAll()
        {
            return Ok(_databaseContext.Projects.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Task> Get(int id)
        {
            var project = _databaseContext.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public ActionResult<int> Add(string name)
        {
            var projectResult = _projectService.AddProject(name);
            return ActionResultHelper<int>.GetActionResult(projectResult);
        }

        [HttpPost("{id}/rename/{name}")]
        public ActionResult<ProjectModel> Rename(int id, string name)
        {
            var projectResult = _projectService.Rename(id, name);
            return ActionResultHelper<ProjectModel>.GetActionResult(projectResult, false);
        }
    }
}