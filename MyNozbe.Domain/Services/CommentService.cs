using System.Threading.Tasks;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Services
{
    public class CommentService
    {
        private readonly IDbOperations<CommentModel> _commentModelDbOperations;

        public CommentService(IDbOperations<CommentModel> commentModelDbOperations)
        {
            _commentModelDbOperations = commentModelDbOperations;
        }

        public async Task<OperationResult<CommentModel>> AddCommentAsync(int taskId, string content)
        {
            var commentModel = new CommentModel(taskId, content);
            await _commentModelDbOperations.AddAsync(commentModel);
            return OperationResult<CommentModel>.Ok();
        }
    }
}
