using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace MyNozbe.API.E2ETests
{
    public class TaskControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private const int TaskId = 1;
        private const int NotExistingTaskId = 9999;

        private readonly WebApplicationFactory<Startup> _factory;

        public TaskControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [AutoData]
        public async Task Create_ShouldAddTaskAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var taskResult = await GetResult<TaskTestModel>(response);
            Assert.Equal(taskName, taskResult.Name);
        }

        [Theory]
        [AutoData]
        public async Task CreateWithTooLongName_ShouldFailAsync([MinLength(31)] string taskName)
        {
            // Arrange
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task CreateWithTooShortName_ShouldFailAsync([MaxLength(2)] string taskName)
        {
            // Arrange
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetWithoutParameter_ShouldReturnAllTasksAsync([MaxLength(30)] string task1Name,
            [MaxLength(30)] string task2Name)
        {
            // Arrange
            const string url = "task";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(task1Name, client);
            await CreateTestTaskAsync(task2Name, client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tasksResult = await GetResult<List<TaskTestModel>>(response);
            Assert.NotNull(tasksResult.FirstOrDefault(x => x.Name == task1Name));
            Assert.NotNull(tasksResult.FirstOrDefault(x => x.Name == task2Name));
        }

        [Theory]
        [AutoData]
        public async Task GetWithParameter_ShouldReturnTaskAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var url = $"task/{TaskId}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(taskName, client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var taskResult = await GetResult<TaskTestModel>(response);
            Assert.Equal(TaskId, taskResult.Id);
        }

        [Theory]
        [AutoData]
        public async Task MarkClosed_ShouldChangeIsCompletedToTrueAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var url = $"task/{TaskId}/close";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(taskName, client);

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{TaskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(TaskId, taskResult.Id);
            Assert.True(taskResult.IsCompleted);
        }

        [Theory]
        [AutoData]
        public async Task MarkOpened_ShouldChangeIsCompletedToFalseAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var url = $"task/{TaskId}/open";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(taskName, client);

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{TaskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(TaskId, taskResult.Id);
            Assert.False(taskResult.IsCompleted);
        }

        [Theory]
        [AutoData]
        public async Task Rename_ShouldChangeNameToNewOneAsync([MaxLength(30)] string taskName,
            [MaxLength(30)] string newTaskName)
        {
            // Arrange
            var url = $"task/{TaskId}/rename/{newTaskName}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(taskName, client);

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{TaskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(TaskId, taskResult.Id);
            Assert.Equal(newTaskName, taskResult.Name);
        }

        [Theory]
        [AutoData]
        public async Task RenameWithTooLongName_ShouldFailAsync([MaxLength(30)] string taskName,
            [MinLength(31)] string newTaskName)
        {
            // Arrange
            var url = $"task/{TaskId}/rename/{newTaskName}";
            var client = _factory.CreateClient();
            await CreateTestTaskAsync(taskName, client);

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        public async Task MarkClosedNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/{NotExistingTaskId}/close";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MarkOpenedNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/{NotExistingTaskId}/open";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task RenameNotExistingTask_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = $"task/{NotExistingTaskId}/rename/test";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

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
    }
}