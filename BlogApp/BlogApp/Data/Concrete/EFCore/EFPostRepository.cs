using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EFPostRepository : EFBaseRepository<Post>, IPostRepository
    {
        public EFPostRepository(BlogContext context) : base(context)
        {

        }

        public async Task CreatePost(Post post)
        {
            _Context.Posts.Add(post);
            await _Context.SaveChangesAsync();
        }

        public async Task CreatePost(Post post, int[] tagIds)
        {
            post.Tags = await _Context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
            _Context.Posts.Add(post);
            await _Context.SaveChangesAsync();
        }

        public async Task DeletePost(Post entity)
        {
            var post = await _Context.Posts.FirstOrDefaultAsync(p => p.PostId == entity.PostId);
            if (post != null)
            {
                post.IsDelete = true;
                await _Context.SaveChangesAsync();
            }
        }

        public async Task EditPost(Post entity)
        {
            var post = await _Context.Posts.FirstOrDefaultAsync(p => p.PostId == entity.PostId);
            if (post != null)
            {
                post.Title = entity.Title;
                post.Description = entity.Description;
                post.Url = entity.Url;
                post.Content = entity.Content;
                post.Image = entity.Image;
                post.IsActive = entity.IsActive;
                await _Context.SaveChangesAsync();
            }
        }

        public async Task EditPost(Post entity, int[] tagIds)
        {
            var post = await _Context.Posts.Include(t => t.Tags).FirstOrDefaultAsync(p => p.PostId == entity.PostId);
            if (post != null)
            {
                post.Title = entity.Title;
                post.Description = entity.Description;
                post.Url = entity.Url;
                post.Content = entity.Content;
                post.Image = entity.Image;
                post.IsActive = entity.IsActive;

                post.Tags = _Context.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToList();
                await _Context.SaveChangesAsync();
            }

        }

    }
}
