using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using Xunit;

namespace MyNozbe.Domain.UnitTests
{
    public class CommentServiceTest
    {
        [Theory]
        [AutoMoqData]
        public async Task AddComment_ShouldCallAddMethodAsync(
            [Frozen] Mock<IDbOperations<CommentModel>> commentModelDbOperationsMock,
            CommentService commentService)
        {
            var commentResult = await commentService.AddCommentAsync(1, "testContent");

            commentModelDbOperationsMock.Verify(
                x => x.AddAsync(It.Is<CommentModel>(y =>
                    y.TaskId == 1 && y.Content =="testContent")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateComment_ShouldCallUpdateMethodAsync(
            Mock<IDbOperations<TaskModel>> taskModelDbOperationMock,
            [Frozen] Mock<IDbOperations<CommentModel>> commentModelDbOperationsMock,
            CommentService commentService)
        {
            taskModelDbOperationMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "testTask", false));
            commentModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new CommentModel(1, 1, "testContent"));

            var commentResult = await commentService.UpdateCommentAsync(1, "newTestContent");

            commentModelDbOperationsMock.Verify(
                x => x.UpdateAsync(It.Is<CommentModel>(y =>
                    y.TaskId == 1 && y.Content == "newTestContent")),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteComment_ShouldCallDeleteMethodAsync(
            Mock<IDbOperations<TaskModel>> taskModelDbOperationMock,
            [Frozen] Mock<IDbOperations<CommentModel>> commentModelDbOperationsMock,
            CommentService commentService)
        {
            taskModelDbOperationMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new TaskModel(1, "testTask", false));
            commentModelDbOperationsMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new CommentModel(1, 1, "testContent"));

            var commentResult = await commentService.DeleteCommentAsync(1);

            commentModelDbOperationsMock.Verify(
                x => x.DeleteAsync(It.Is<CommentModel>(y =>
                    y.TaskId == 1)),
                Times.Once);
        }
    }
}