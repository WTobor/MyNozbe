using System;
using System.Threading.Tasks;
using MyNozbe.Database.Models;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace MyNozbe.Database.Repositories
{
    public class CommentRepository : IDbOperations<CommentModel>
    {
        private readonly DatabaseContext _databaseContext;

        public CommentRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> AddAsync(CommentModel model)
        {
            var project = new Comment(model.TaskId, model.Content, DateTimeOffset.Now);
            await _databaseContext.Comments.AddAsync(project);
            await _databaseContext.SaveChangesAsync();
            return project.Id;
        }

        public async Task<CommentModel> GetAsync(int id)
        {
            var comment = await _databaseContext.Comments.FindAsync(id);
            return new CommentModel(comment.TaskId, comment.Content);
        }

        public async Task UpdateAsync(CommentModel model)
        {
            var comment = await _databaseContext.Comments.FindAsync(model.Id);
            comment.Content = model.Content;
            await _databaseContext.SaveChangesAsync();
        }
    }
}