using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EFCommentRepository : EFBaseRepository<Comment>, ICommentRepository
    {
        public EFCommentRepository(BlogContext Context) : base(Context)
        {
        }

        public async Task CreateComment(Comment comment)
        {
            _Context.Comments.Add(comment);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteComment(Comment comment)
        {
            _Context.Remove(comment);
            await _Context.SaveChangesAsync();
        }
    }
}
