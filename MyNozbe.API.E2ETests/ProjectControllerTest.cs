using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class ProjectControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ProjectControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [AutoData]
        public async Task Add_ShouldReturnIdBiggerThanZeroAsync([MaxLength(20)] string projectName)
        {
            // Arrange
            var url = $"project?name={projectName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var projectResult = await ResponseHelper.GetResult<int>(response);
            projectResult.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("aa")]
        [InlineData("this_is_too_long_name_for_project")]
        public async Task Add_WhenInvalidName_ShouldReturnBadRequestAsync(string projectName)
        {
            // Arrange
            var url = $"project?name={projectName}";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [AutoData]
        public async Task GetAll_ShouldReturnAddedProjectsAsync([MaxLength(20)] string project1Name, [MaxLength(20)] string project2Name)
        {
            // Arrange
            var url = "project";
            var client = _factory.CreateClient();
            var project1Id = await AddTestProjectAsync(project1Name, client);
            var project2Id = await AddTestProjectAsync(project2Name, client);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var projectResult = await ResponseHelper.GetResult<List<ProjectTestModel>>(response);
            projectResult.Should().Contain(x => x.Id == project1Id);
            projectResult.Should().Contain(x => x.Id == project2Id);
        }

        [Theory]
        [AutoData]
        public async Task Get_ShouldReturnProjectWithCorrectIdAsync([MaxLength(20)] string projectName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var projectId = await AddTestProjectAsync(projectName, client);
            var url = $"project/{projectId}";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var projectResult = await ResponseHelper.GetResult<ProjectTestModel>(response);
            projectResult.Id.Should().Be(projectId);
        }

        [Theory]
        [AutoData]
        public async Task Rename_ShouldChangeNameToNewOneAsync([MaxLength(20)] string projectName, [MaxLength(20)] string newProjectName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var projectId = await AddTestProjectAsync(projectName, client);
            var url = $"project/{projectId}/rename/{newProjectName}";

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var projectResponse = await client.GetAsync($"project/{projectId}");

            var projectResult = await ResponseHelper.GetResult<ProjectTestModel>(projectResponse);
            projectResult.Id.Should().Be(projectId);
            projectResult.Name.Should().Be(newProjectName);
        }

        [Theory]
        [AutoData]
        public async Task Rename_WhenTooLongName_ShouldReturnBadRequestAsync([MaxLength(20)] string projectName, [MinLength(31)] string newProjectName)
        {
            // Arrange
            var client = _factory.CreateClient();
            var projectId = await AddTestProjectAsync(projectName, client);
            var url = $"project/{projectId}/rename/{newProjectName}";

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static async Task<int> AddTestProjectAsync(string name, HttpClient client)
        {
            var url = $"project?name={name}";
            var response = await client.PostAsync(url, null);
            return await ResponseHelper.GetResult<int>(response);
        }
    }
}