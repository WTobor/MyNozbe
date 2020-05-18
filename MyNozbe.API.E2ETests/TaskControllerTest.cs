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
        private const int NotExistingTaskId = 9999;

        private readonly WebApplicationFactory<Startup> _factory;

        public TaskControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [AutoData]
        public async Task Add_ShouldReturnIdBiggerThanZeroAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var taskResult = await GetResult<int>(response);
            Assert.True(taskResult > 0);
        }

        [Theory]
        [AutoData]
        public async Task AddWithTooLongName_ShouldFailAsync([MinLength(31)] string taskName)
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
        public async Task AddWithTooShortName_ShouldFailAsync([MaxLength(2)] string taskName)
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
            var task1Id = await AddTestTaskAsync(task1Name, client);
            var task2Id = await AddTestTaskAsync(task2Name, client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tasksResult = await GetResult<List<TaskTestModel>>(response);
            Assert.Contains(task1Id, tasksResult.Select(x => x.Id));
            Assert.Contains(task2Id, tasksResult.Select(x => x.Id));
        }

        [Theory]
        [AutoData]
        public async Task GetWithParameter_ShouldReturnTaskAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var taskResult = await GetResult<TaskTestModel>(response);
            Assert.Equal(taskId, taskResult.Id);
        }

        [Theory]
        [AutoData]
        public async Task MarkClosed_ShouldChangeIsCompletedToTrueAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/close";

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{taskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(taskId, taskResult.Id);
            Assert.True(taskResult.IsCompleted);
        }

        [Theory]
        [AutoData]
        public async Task MarkOpened_ShouldChangeIsCompletedToFalseAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/open";

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{taskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(taskId, taskResult.Id);
            Assert.False(taskResult.IsCompleted);
        }

        [Theory]
        [AutoData]
        public async Task Rename_ShouldChangeNameToNewOneAsync([MaxLength(30)] string taskName,
            [MaxLength(30)] string newTaskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/rename/{newTaskName}";

            // Act
            var operationResponse = await client.PutAsync(url, null);
            var taskResponse = await client.GetAsync($"task/{taskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, operationResponse.StatusCode);

            var taskResult = await GetResult<TaskTestModel>(taskResponse);
            Assert.Equal(taskId, taskResult.Id);
            Assert.Equal(newTaskName, taskResult.Name);
        }

        [Theory]
        [AutoData]
        public async Task RenameWithTooLongName_ShouldFailAsync([MaxLength(30)] string taskName,
            [MinLength(31)] string newTaskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/rename/{newTaskName}";

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

        private static async Task<int> AddTestTaskAsync(string name, HttpClient client)
        {
            var url = $"task?name={name}";
            var response = await client.PostAsync(url, null);
            return await GetResult<int>(response);
        }
    }
}