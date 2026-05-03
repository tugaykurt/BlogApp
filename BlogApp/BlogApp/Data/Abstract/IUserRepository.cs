using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> GetAll {  get; }
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task UpdateUserPassword(User user);
    }
}
