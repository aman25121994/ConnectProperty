using Microsoft.EntityFrameworkCore;
using PropertyConnect.Models;

namespace PropertyConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Property> Properties => Set<Property>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Properties");
                entity.Property(p => p.Price).HasColumnType("REAL");
                entity.Property(p => p.CreatedAt).HasColumnType("TEXT");
            });
        }
    }
}
