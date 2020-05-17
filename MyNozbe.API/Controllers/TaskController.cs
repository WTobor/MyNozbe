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
            return Ok(_databaseContext.Tasks.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Task> Get(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public ActionResult<TaskModel> Add(string name)
        {
            var taskResult = _taskService.AddTask(name);
            return new ActionResultHelper<TaskModel>().GetActionResult(taskResult);
        }

        [HttpPut("rename/{id}&{name}")]
        public ActionResult<TaskModel> Rename(int id, string name)
        {
            var taskResult = _taskService.Rename(id, name);
            return new ActionResultHelper<TaskModel>().GetActionResult(taskResult, false);
        }

        [HttpPut("close/{id}")]
        public ActionResult<TaskModel> MarkClosed(int id)
        {
            var taskResult = _taskService.MarkTaskAsClosed(id);
            return new ActionResultHelper<TaskModel>().GetActionResult(taskResult, false);
        }

        [HttpPut("open/{id}")]
        public ActionResult<TaskModel> MarkOpened(int id)
        {
            var taskResult = _taskService.MarkTaskAsOpened(id);
            return new ActionResultHelper<TaskModel>().GetActionResult(taskResult, false);
        }
    }
}