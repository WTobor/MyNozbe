using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNozbe.Database;
using MyNozbe.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyNozbe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly DatabaseContext _databaseContext;

        public TaskController(ILogger<TaskController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Task>> Get()
        {
            return _databaseContext.Tasks.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Task> Get(int id)
        {
            return _databaseContext.Tasks.Find(id);
        }
 
        [HttpPost]
        public ActionResult<Task> Add(string name)
        {
            var task = new Task()
            {
                Name = name
            };
            _databaseContext.Tasks.Add(task);
            _databaseContext.SaveChanges();
            return task;
        }

        [HttpPut("close/{id}")]
        public ActionResult MarkClosed(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            task.IsCompleted = true;
            _databaseContext.SaveChanges();

            return NoContent();
        }

        [HttpPut("open/{id}")]
        public ActionResult MarkOpened(int id)
        {
            var task = _databaseContext.Tasks.Find(id);
            if(task == null)
            {
                return NotFound();
            }
            task.IsCompleted = false;
            _databaseContext.SaveChanges();

            return NoContent();
        }
    }
}
