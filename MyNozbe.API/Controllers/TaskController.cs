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
        public ActionResult<IEnumerable<Task>> GetAll()
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
        public ActionResult<int> Add(string name)
        {
            var taskResult = _taskService.AddTask(name);
            return ActionResultHelper<int>.GetActionResult(taskResult);
        }

        [HttpPut("{id}/rename/{name}")]
        public ActionResult<TaskModel> Rename(int id, string name)
        {
            var taskResult = _taskService.Rename(id, name);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPut("{id}/close")]
        public ActionResult<TaskModel> MarkClosed(int id)
        {
            var taskResult = _taskService.MarkTaskAsClosed(id);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPut("{id}/open")]
        public ActionResult<TaskModel> MarkOpened(int id)
        {
            var taskResult = _taskService.MarkTaskAsOpened(id);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPost("{id}/assign/project/{projectId}")]
        public ActionResult<TaskModel> AssignTask(int id, int projectId)
        {
            var taskResult = _taskService.AssignProject(id, projectId);

            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }
    }
}