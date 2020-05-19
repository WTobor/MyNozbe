using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using MyNozbe.Domain.Validators;
using Xunit;

namespace MyNozbe.Domain.UnitTests
{
    public class TaskServiceTest
    {
        [Theory]
        [AutoMoqData]
        public void AddTask_ShouldCallAddMethod([Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModelValidator>> taskModelValidatorMock,
            TaskService taskService)
        {
            taskModelValidatorMock.Setup(x => x.Validate(It.IsAny<TaskModel>())).Returns(new ValidationResult());
            taskService.AddTask("test");

            taskModelDbOperationsMock.Verify(x => x.Add(It.Is<TaskModel>(y => y.Name == "test")), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void AddTask_WhenValidationFailure_ShouldNotCallAddMethod(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            taskModelValidatorMock.Setup(x => x.Validate(It.IsAny<TaskModel>())).Returns(valResult);

            taskService.AddTask("test");

            taskModelDbOperationsMock.Verify(x => x.Add(It.Is<TaskModel>(y => y.Name == "test")), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void Rename_ShouldCallUpdateMethod([Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new TaskModel(1, "test", false));

            taskService.Rename(1, "newName");

            taskModelDbOperationsMock.Verify(x => x.Update(It.Is<TaskModel>(y => y.Name == "newName")), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Rename_WhenValidationFailure_ShouldNotCallAddMethod(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            [Frozen] Mock<IValidator<TaskModel>> taskModelValidatorMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new TaskModel(1, "test", false));
            var valResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "aaa")
            });
            taskModelValidatorMock.Setup(x => x.Validate(It.IsAny<TaskModel>())).Returns(valResult);

            taskService.Rename(1, "test");

            taskModelDbOperationsMock.Verify(x => x.Add(It.Is<TaskModel>(y => y.Name == "test")), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void MarkTaskAsOpened_ShouldCallUpdateMethod(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new TaskModel(1, "test", false));

            taskService.MarkTaskAsOpened(1);

            taskModelDbOperationsMock.Verify(
                x => x.Update(It.Is<TaskModel>(y => y.Name == "test" && y.IsCompleted == false)), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void MarkTaskAsClosed_ShouldCallUpdateMethod(
            [Frozen] Mock<IDbOperations<TaskModel>> taskModelDbOperationsMock,
            TaskService taskService)
        {
            taskModelDbOperationsMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new TaskModel(1, "test", false));

            taskService.MarkTaskAsClosed(1);

            taskModelDbOperationsMock.Verify(x => x.Update(It.Is<TaskModel>(y => y.Name == "test" && y.IsCompleted)),
                Times.Once);
        }
    }
}