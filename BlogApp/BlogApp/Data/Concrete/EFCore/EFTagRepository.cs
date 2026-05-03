using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EFTagRepository : EFBaseRepository<Tag>, ITagRepository
    {
        public EFTagRepository(BlogContext context) : base(context)
        {
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            _Context.Tags.Add(tag);
            await _Context.SaveChangesAsync();
            return tag;
        }
    }
}
