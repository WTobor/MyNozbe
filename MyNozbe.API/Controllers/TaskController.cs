using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNozbe.Database;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using Task = MyNozbe.Database.Models.Task;

namespace MyNozbe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly CommentService _commentService;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<TaskController> _logger;
        private readonly TaskService _taskService;

        public TaskController(ILogger<TaskController> logger, DatabaseContext databaseContext, TaskService taskService,
            CommentService commentService)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _taskService = taskService;
            _commentService = commentService;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<Task>>> GetAllAsync()
        {
            return Ok(await _databaseContext.Tasks.ToListAsync());
        }

        [HttpGet("{id}")]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetAsync(int id)
        {
            var task = await _databaseContext.Tasks
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddAsync(string name)
        {
            var taskResult = await _taskService.AddTaskAsync(name);
            return ActionResultHelper<int>.GetActionResult(taskResult);
        }

        [HttpPut("{id}/rename/{name}")]
        public async Task<ActionResult<TaskModel>> RenameAsync(int id, string name)
        {
            var taskResult = await _taskService.RenameAsync(id, name);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPut("{id}/close")]
        public async Task<ActionResult<TaskModel>> MarkClosedAsync(int id)
        {
            var taskResult = await _taskService.MarkTaskAsClosedAsync(id);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPut("{id}/open")]
        public async Task<ActionResult<TaskModel>> MarkOpenedAsync(int id)
        {
            var taskResult = await _taskService.MarkTaskAsOpenedAsync(id);
            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPost("{id}/assign/project/{projectId}")]
        public async Task<ActionResult<TaskModel>> AssignProjectAsync(int id, int projectId)
        {
            var taskResult = await _taskService.AssignProjectAsync(id, projectId);

            return ActionResultHelper<TaskModel>.GetActionResult(taskResult, false);
        }

        [HttpPost("{id}/comment/{content}")]
        public async Task<ActionResult<CommentModel>> AddCommentAsync(int id, string content)
        {
            var commentResult = await _commentService.AddCommentAsync(id, content);

            return ActionResultHelper<CommentModel>.GetActionResult(commentResult, false);
        }
    }
}