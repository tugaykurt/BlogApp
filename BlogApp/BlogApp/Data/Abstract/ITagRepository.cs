using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface ITagRepository
    {
        IQueryable<Tag> GetAll { get; }
        Task<Tag> CreateTag(Tag tag);
    }
}
