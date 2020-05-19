using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNozbe.Database;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Models;

namespace MyNozbe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ILogger<ProjectController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
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
            var project = new Project(name, DateTimeOffset.Now);
            _databaseContext.Projects.Add(project);
            _databaseContext.SaveChanges();

            var result = new OperationResult<int>
            {
                ResultObject = project.Id
            };
            return ActionResultHelper<int>.GetActionResult(result);
        }
    }
}