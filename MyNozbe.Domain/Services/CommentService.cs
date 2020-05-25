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

        public async Task<OperationResult<int>> AddCommentAsync(int taskId, string content)
        {
            var commentModel = new CommentModel(taskId, content);
            var commentId = await _commentModelDbOperations.AddAsync(commentModel);
            return OperationResult<int>.Ok(commentId);
        }

        public async Task<OperationResult<CommentModel>> UpdateCommentAsync(int commentId, string content)
        {
            var comment = await _commentModelDbOperations.GetAsync(commentId);
            if (comment == null)
            {
                return OperationResult<CommentModel>.NotFound();
            }

            comment.Content = content;

            await _commentModelDbOperations.UpdateAsync(comment);
            return OperationResult<CommentModel>.Ok();
        }
    }
}