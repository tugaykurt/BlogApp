namespace BlogApp.Data.Abstract
{
    public interface IBaseRepository<TEntity>
    {
        IQueryable<TEntity> GetAll {  get; }
    }
}
