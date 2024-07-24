using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public class ArticlesContext:DbContext
    {
        public ArticlesContext(DbContextOptions<ArticlesContext> options):base(options)
        {
        }
        public DbSet<Article> Articles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("article");
        }
    }
}
