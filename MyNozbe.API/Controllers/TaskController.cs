using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyNozbe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;

        public TaskController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Task Get()
        {
            return new Task("TestTask");
        }
    }
}
