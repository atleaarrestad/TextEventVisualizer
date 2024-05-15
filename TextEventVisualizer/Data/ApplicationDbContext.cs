using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;

namespace TextEventVisualizer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TimelineChunk> TimelineChunks { get; set; }
        public DbSet<Timeline> Timelines { get; set; }
        public DbSet<TimelineRequest> TimelineRequests { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // SQLite database relations setup
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Category);

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Date);

            modelBuilder.Entity<Timeline>()
                .HasMany(t => t.TimelineChunks)
                .WithOne(tc => tc.Timeline)
                .HasForeignKey(tc => tc.TimelineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Timeline>()
                .HasOne(t => t.TimelineRequest)
                .WithOne(tr => tr.Timeline)
                .HasForeignKey<Timeline>(t => t.TimelineRequestId);

            modelBuilder.Entity<TimelineChunk>()
                .HasMany(tc => tc.Events)
                .WithOne(e => e.TimelineChunk)
                .HasForeignKey(e => e.TimelineChunkId);

            modelBuilder.Entity<TimelineChunk>()
                .HasOne(tc => tc.Article)
                .WithMany()
                .HasForeignKey(tc => tc.ArticleId);

            modelBuilder.Entity<TimelineRequest>().OwnsOne(tr => tr.ArticleClusterSearchPositiveBias, bias =>
            {
                bias.WithOwner();
                bias.Property(b => b.Force).HasColumnName("PositiveBiasForce");
            });

            modelBuilder.Entity<TimelineRequest>().OwnsOne(tr => tr.ArticleClusterSearchNegativeBias, bias =>
            {
                bias.WithOwner();
                bias.Property(b => b.Force).HasColumnName("NegativeBiasForce");
            });
        }
    }
}
