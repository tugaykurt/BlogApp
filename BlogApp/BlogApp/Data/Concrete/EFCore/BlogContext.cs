using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                 .HasOne(c => c.Post)
                 .WithMany(u => u.Comments)
                 .HasForeignKey(c => c.PostId)
                 .OnDelete(DeleteBehavior.Cascade);
            // Bu onmodelcreating'i şu yüzden oluşturduk, sql server iki tane multiple cascade path yani 1 den fazla otomatik silme yolunu kabul etmiyor. Bu durumu migration ve modelsnapshot'ta düzeltirsek bu durumda'da sistem kendi oluşturduğu migration'u arıyor. O yüzden migration oluşturmadan önce modelBuilder yapıp daha sonra migration oluşturmamız gerekli.
            // Sorun şu sql server, bir user silinince, user'ın comment'lerinin otomatik silinmesini ve bir post silinince, post'un comment'lerinin otomatik silinmesini kabul etmiyor. Çünkü hangisini silineceği konusunda karışıklık çıkmasını engellemiş oluyor. Biz burada eğer bir user'ı sileceksen o user'ın comment'lerini manuel sileceğiz demiş oluyoruz. Bir post silindiğinde de o posta ait comment'lar otomatik silinsin demiş oluyoruz.
            // Migration 2 foreignkey olduğunda sql server'a göre modeller arasında doğru bir bağlantı oluşturamıyor. O yüzden burada migration'a yol göstermek için onmodelcreating işlemi gerçekleştirdik.
        }
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Comment> Comments => Set<Comment>();
    }
}
