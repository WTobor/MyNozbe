using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace MyNozbe.API.E2ETests
{
    public class TaskControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public TaskControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private readonly WebApplicationFactory<Startup> _factory;
        private const int TaskId = 1;
        private const int NotExistingTaskId = 9999;

        private static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }

        private static async Task CreateTestTaskAsync(string name, HttpClient client)
        {
            var url = $"task?name={name}";
            await client.PostAsync(url, null);
        }

        [Fact]
        public async Task Create_ShouldAddTaskAsync()
        {
            // Arrange
            const string taskName = "TestTask";
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            var task = await GetResult<Database.Models.Task>(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(taskName, task.Name);
        }

        [Fact]
        public async Task GetWithoutParameter_ShouldReturnAllTasksAsync()
        {
            // Arrange
            const string url = "task";
            const string task1Name = "task1";
            const string task2Name = "task2";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(task1Name, client);
            await CreateTestTaskAsync(task2Name, client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tasks = await GetResult<List<Database.Models.Task>>(response);
            Assert.NotNull(tasks.FirstOrDefault(x => x.Name == task1Name));
            Assert.NotNull(tasks.FirstOrDefault(x => x.Name == task2Name));
        }

        [Fact]
        public async Task GetWithParameter_ShouldReturnTaskAsync()
        {
            // Arrange
            var url = $"task/{TaskId}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync("task1", client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var task = await GetResult<Database.Models.Task>(response);
            Assert.Equal(TaskId, task.Id);
        }

        [Fact]
        public async Task GetWithParameterNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/{NotExistingTaskId}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MarkClosed_ShouldChangeIsCompletedToTrueAsync()
        {
            // Arrange
            var url = $"task/close/{TaskId}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync("task1", client);

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{TaskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var task = await GetResult<Database.Models.Task>(taskResponse);
            Assert.Equal(TaskId, task.Id);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public async Task MarkClosedNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/close/{NotExistingTaskId}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MarkOpened_ShouldChangeIsCompletedToFalseAsync()
        {
            // Arrange
            var url = $"task/open/{TaskId}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync("task1", client);

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{TaskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var task = await GetResult<Database.Models.Task>(taskResponse);
            Assert.Equal(TaskId, task.Id);
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public async Task MarkOpenedNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/open/{NotExistingTaskId}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}