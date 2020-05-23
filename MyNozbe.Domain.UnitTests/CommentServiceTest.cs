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
    }
}