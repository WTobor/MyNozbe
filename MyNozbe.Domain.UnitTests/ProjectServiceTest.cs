using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation;
using FluentValidation.Results;
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
            [Frozen] Mock<IValidator<ProjectModel>> projectModelValidatorMock,
            ProjectService projectService)
        {
            projectModelValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            await projectService.AddProjectAsync("test");

            projectModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<ProjectModel>(y => y.Name == "test")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddProject_WhenValidationFailure_ShouldNotCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<ProjectModel>> projectModelDbOperationsMock,
            [Frozen] Mock<IValidator<ProjectModel>> projectModelValidatorMock,
            ProjectService projectService)
        {
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            projectModelValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(valResult);

            await projectService.AddProjectAsync("test");

            projectModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<ProjectModel>(y => y.Name == "test")),
                Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public async Task RenameProject_ShouldCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<ProjectModel>> projectModelDbOperationsMock,
            [Frozen] Mock<IValidator<ProjectModel>> projectModelValidatorMock,
            ProjectService projectService)
        {
            projectModelValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            projectModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ProjectModel(1, "test", null));

            await projectService.RenameAsync(1, "newName");

            projectModelDbOperationsMock.Verify(x => x.UpdateAsync(It.Is<ProjectModel>(y => y.Name == "newName")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task RenameProject_WhenValidationFailed_ShouldNotCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<ProjectModel>> projectModelDbOperationsMock,
            [Frozen] Mock<IValidator<ProjectModel>> projectModelValidatorMock,
            ProjectService projectService)
        {
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            projectModelValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(valResult);

            projectModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ProjectModel(1, "test", null));
            await projectService.RenameAsync(1, "newName");

            projectModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<ProjectModel>(y => y.Name == "newName")),
                Times.Never);
        }
    }
}