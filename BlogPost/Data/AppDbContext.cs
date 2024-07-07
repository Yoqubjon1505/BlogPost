using BlogPost.Model;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BlogPostModel> BlogPosts { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<BlogPost>()
        //        .Property(b => b.Id)
        //        .ValueGeneratedOnAdd();
        //}
    }
}
