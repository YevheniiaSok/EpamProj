using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace WebApplication2.Models
{
    public class ArticlesContext : IdentityDbContext<AspNetUsers>
    {
        public ArticlesContext(DbContextOptions<ArticlesContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }
        public DbSet<Article> Articles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed a default user
            var hasher = new PasswordHasher<IdentityUser>();
            modelBuilder.HasDefaultSchema("article");
           
            var passwordHash = hasher.HashPassword(null, "P@f0iujKTI9pWIPqRsem~");

            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = "default-user-id",
                    UserName = "default@example.com",
                    NormalizedUserName = "DEFAULT@EXAMPLE.COM",
                    Email = "default@example.com",
                    NormalizedEmail = "DEFAULT@EXAMPLE.COM",
                    PasswordHash = passwordHash,
                    EmailConfirmed = false,
                    LockoutEnabled = true
                });         
           
            
        }
    }
    
}
