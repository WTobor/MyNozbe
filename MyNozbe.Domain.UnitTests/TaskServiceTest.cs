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
    public class TaskServiceTest
    {
        [Theory]
        [AutoMoqData]
        public async Task AddTask_ShouldCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            taskModelValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<TaskModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            await taskService.AddTaskAsync("test");

            taskModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<TaskModel>(y => y.Name == "test")), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddTask_WhenValidationFailure_ShouldNotCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            taskModelValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<TaskModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(valResult);

            await taskService.AddTaskAsync("test");

            taskModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<TaskModel>(y => y.Name == "test")), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public async Task Rename_ShouldCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            taskModelValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<TaskModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            taskModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "test", false));

            await taskService.RenameAsync(1, "newName");

            taskModelDbOperationsMock.Verify(x => x.UpdateAsync(It.Is<TaskModel>(y => y.Name == "newName")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task Rename_WhenValidationFailure_ShouldNotCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "test", false));
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            taskModelValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<TaskModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(valResult);

            await taskService.RenameAsync(1, "test");

            taskModelDbOperationsMock.Verify(x => x.AddAsync(It.Is<TaskModel>(y => y.Name == "test")), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public async Task MarkTaskAsOpened_ShouldCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "test", false));

            await taskService.MarkTaskAsOpenedAsync(1);

            taskModelDbOperationsMock.Verify(
                x => x.UpdateAsync(It.Is<TaskModel>(y => y.Name == "test" && y.IsCompleted == false)), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task MarkTaskAsClosed_ShouldCallUpdateMethodAsync(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "test", false));

            await taskService.MarkTaskAsClosedAsync(1);

            taskModelDbOperationsMock.Verify(
                x => x.UpdateAsync(It.Is<TaskModel>(y => y.Name == "test" && y.IsCompleted)),
                Times.Once);
        }
    }
}