using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyNozbe.API.E2ETests.TestModels;
using Xunit;

namespace MyNozbe.API.E2ETests
{
    public class TaskControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var taskResult = await ResponseHelper.GetResult<int>(response);
            taskResult.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("aa")]
        [InlineData("this_is_too_long_name_for_task_")]
        public async Task Add_WhenInvalidName_ShouldReturnBadRequestAsync(string taskName)
        {
            // Arrange
            var url = $"task?name={taskName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [AutoData]
        public async Task GetAll_ShouldReturnAddedTasksAsync([MaxLength(30)] string task1Name,
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var tasksResult = await ResponseHelper.GetResult<List<TaskTestModel>>(response);
            tasksResult.Should().Contain(x => x.Id == task1Id);
            tasksResult.Should().Contain(x => x.Id == task2Id);
        }

        [Theory]
        [AutoData]
        public async Task Get_ShouldReturnTaskWithCorrectIdAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(response);
            taskResult.Id.Should().Be(taskId);
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

            // Assert

            var taskResponse = await client.GetAsync($"task/{taskId}");
            operationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(taskResponse);
            taskResult.Id.Should().Be(taskId);
            taskResult.IsCompleted.Should().BeTrue();
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

            // Assert
            var taskResponse = await client.GetAsync($"task/{taskId}");
            operationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(taskResponse);
            taskResult.Id.Should().Be(taskId);
            taskResult.IsCompleted.Should().BeFalse();
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

            // Assert
            operationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var taskResponse = await client.GetAsync($"task/{taskId}");

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(taskResponse);
            taskResult.Id.Should().Be(taskId);
            taskResult.Name.Should().Be(newTaskName);
        }

        [Theory]
        [AutoData]
        public async Task Rename_WhenNameIsTooLong_ShouldReturnBadRequestAsync([MaxLength(30)] string taskName,
            [MinLength(31)] string newTaskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/rename/{newTaskName}";

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_WhenIdIsNotExisting_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = "task/-1";
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task MarkClosed_WhenIdIsNotExisting_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = "task/-1/close";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task MarkOpened_WhenIdIsNotExisting_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = "task/-1/open";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Rename_WhenIdIsNotExisting_ShouldReturnNotFoundAsync()
        {
            // Arrange
            var url = "task/-1/rename/test";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [AutoData]
        public async Task AssignProject_ShouldAssignTaskToProjectAsync([MaxLength(20)] string projectName,
            [MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var projectId = await AddTestProjectAsync(projectName, client);
            var url = $"task/{taskId}/assign/project/{projectId}";

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var testResponse = await client.GetAsync($"task/{taskId}");

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(testResponse);
            taskResult.ProjectId.Should().Be(projectId);
        }

        [Theory]
        [AutoData]
        public async Task AddComment_ShouldAddCommentToTaskAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var url = $"task/{taskId}/comment/testComment";

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var commentId = await ResponseHelper.GetResult<int>(response);
            var testResponse = await client.GetAsync($"task/{taskId}");

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(testResponse);
            taskResult.Comments.Count.Should().Be(1);
            taskResult.Comments.First().Id.Should().Be(commentId);
        }

        [Theory]
        [AutoData]
        public async Task UpdateComment_ShouldUpdateCommentContentAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var commentId = await AddTestCommentAsync(taskId, "testComment", client);
            var url = $"task/{taskId}/update/comment/{commentId}/newTestComment";

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var testResponse = await client.GetAsync($"task/{taskId}");

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(testResponse);
            taskResult.Comments.First().Content.Should().Be("newTestComment");
        }

        [Theory]
        [AutoData]
        public async Task DeleteComment_ShouldDeleteCommentAsync([MaxLength(30)] string taskName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var taskId = await AddTestTaskAsync(taskName, client);
            var commentId = await AddTestCommentAsync(taskId, "testComment", client);
            var url = $"task/{taskId}/delete/comment/{commentId}";

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var testResponse = await client.GetAsync($"task/{taskId}");

            var taskResult = await ResponseHelper.GetResult<TaskTestModel>(testResponse);
            taskResult.Comments.Count.Should().Be(0);
        }

        private static async Task<int> AddTestProjectAsync(string projectName, HttpClient client)
        {
            var url = $"project?name={projectName}";
            var response = await client.PostAsync(url, null);
            return await ResponseHelper.GetResult<int>(response);
        }

        private static async Task<int> AddTestTaskAsync(string name, HttpClient client)
        {
            var url = $"task?name={name}";
            var response = await client.PostAsync(url, null);
            return await ResponseHelper.GetResult<int>(response);
        }

        private static async Task<int> AddTestCommentAsync(int taskId, string content, HttpClient client)
        {
            var url = $"task/{taskId}/comment/testComment";
            var response = await client.PostAsync(url, null);
            return await ResponseHelper.GetResult<int>(response);
        }
    }
}