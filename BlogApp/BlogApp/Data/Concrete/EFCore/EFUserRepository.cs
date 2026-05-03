using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EFUserRepository : EFBaseRepository<User>, IUserRepository
    {
        public EFUserRepository(BlogContext context) : base(context)
        {
        }

        public async Task CreateUser(User user)
        {
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
        }

        public async Task UpdateUser(User entity)
        {
            var user = await _Context.Users.FirstOrDefaultAsync(u => u.UserId == entity.UserId);
            if (user != null)
            {
                user.UserId = entity.UserId;
                user.UserName = entity.UserName;
                user.Name = entity.Name;
                user.Email = entity.Email;
                user.Image = entity.Image;
                await _Context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserPassword(User entity)
        {
            var user = await _Context.Users.FirstOrDefaultAsync(u => u.UserId == entity.UserId);
            if (user != null) 
            {
                user.UserId = entity.UserId;
                user.Password = entity.Password;
                await _Context.SaveChangesAsync();
            }
        }
    }
}
