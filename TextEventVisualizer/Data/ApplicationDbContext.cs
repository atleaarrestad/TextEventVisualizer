using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Category);

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Date);
        }
    }
}
