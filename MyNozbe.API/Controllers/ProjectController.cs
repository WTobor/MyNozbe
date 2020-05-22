using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNozbe.Database;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using Z.EntityFramework.Plus;
using Task = MyNozbe.Database.Models.Task;

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
        public async Task<ActionResult<IEnumerable<Project>>> GetAllAsync()
        {
            return Ok(await _databaseContext.Projects.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetAsync(int id)
        {
            var project = await _databaseContext.Projects
                .IncludeFilter(x => x.Tasks.Where(p => !p.IsCompleted))
                .FirstOrDefaultAsync(f => f.Id == id);
            
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddAsync(string name)
        {
            var projectResult = await _projectService.AddProjectAsync(name);
            return ActionResultHelper<int>.GetActionResult(projectResult);
        }

        [HttpPost("{id}/rename/{name}")]
        public async Task<ActionResult<ProjectModel>> RenameAsync(int id, string name)
        {
            var projectResult = await _projectService.RenameAsync(id, name);
            return ActionResultHelper<ProjectModel>.GetActionResult(projectResult, false);
        }
    }
}