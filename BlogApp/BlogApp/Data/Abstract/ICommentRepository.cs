using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface ICommentRepository
    {
        IQueryable<Comment> GetAll { get; }
        Task CreateComment(Comment comment);
        Task DeleteComment(Comment comment);
    }
}
