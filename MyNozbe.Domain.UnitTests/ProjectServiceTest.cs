using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using Xunit;

namespace MyNozbe.Domain.UnitTests
{
    public class ProjectServiceTest
    {
        [Theory]
        [AutoMoqData]
        public async Task AddProject_ShouldCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<ProjectModel>> projectModelDbOperationsMock,
            ProjectService projectService)
        {
            await projectService.AddProjectAsync("test");

            projectModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<ProjectModel>(y => y.Name == "test")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task RenameProject_ShouldCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<ProjectModel>> projectModelDbOperationsMock,
            ProjectService projectService)
        {
            projectModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ProjectModel(1, "test", null));
            await projectService.RenameAsync(1, "newName");

            projectModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<ProjectModel>(y => y.Name == "newName")),
                Times.Once);
        }
    }
}