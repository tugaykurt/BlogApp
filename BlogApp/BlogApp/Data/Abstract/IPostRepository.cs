using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post> GetAll { get; } // Burada neden IQueryable interface'ini kullanıdık?: IQueryable ile IEnumarable interface'leri birer List türevi ancak aralarında bir fark var. IEnumarable db'de ki postları hepsini çeker daha sonra biz istersek filtreleme(Where(d => d.)) yapar. IQueryable ise ilk olarak filtrelemeyi yapar, daha sonra o filtreye göre verileri çeker. Yani bütün hepsini almak yerine istenilenleri alır.    
        Task CreatePost(Post post);
        Task CreatePost(Post post, int[] tagIds);
        Task EditPost(Post post);
        Task EditPost(Post post, int[] tagIds);
        Task DeletePost(Post post);
    }
}
