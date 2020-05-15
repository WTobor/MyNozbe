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
    public class TaskController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<TaskController> _logger;
        private readonly TaskService _taskService;

        public TaskController(ILogger<TaskController> logger, DatabaseContext databaseContext, TaskService taskService)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _taskService = taskService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Task>> Get()
        {
            return _databaseContext.Tasks.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Task> Get(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPost]
        public ActionResult<TaskModel> Add(string name)
        {
            var task = _taskService.AddTask(name);
            return task;
        }

        [HttpPut("rename/{id}&&{name}")]
        public ActionResult Rename(int id, string name)
        {
            var task = _taskService.Rename(id, name);
            if (task == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("close/{id}")]
        public ActionResult MarkClosed(int id)
        {
            var task = _taskService.MarkTaskAsClosed(id);
            if (task == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("open/{id}")]
        public ActionResult MarkOpened(int id)
        {
            var task = _taskService.MarkTaskAsOpened(id);
            if (task == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}