using BlogApp.Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EFBaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly BlogContext _Context;

        public EFBaseRepository(BlogContext Context)
        {
            _Context = Context;
        }

        public IQueryable<TEntity> GetAll => _Context.Set<TEntity>();
    }
}
