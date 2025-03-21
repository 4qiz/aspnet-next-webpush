using Microsoft.EntityFrameworkCore;
using WebPushApi.Models;

namespace WebPushApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.Endpoint)
                .IsUnique(); // Уникальность подписки
        }
    }
}
