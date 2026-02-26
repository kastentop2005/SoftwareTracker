using SoftwareTracker.Model;
using Microsoft.EntityFrameworkCore;

namespace SoftwareTracker.Data
{
  public class ProductContext(DbContextOptions<ProductContext> options) : DbContext(options)
  {
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVersion> ProductVersions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
      base.OnConfiguring(builder);

      builder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Define relationships
      builder.Entity<Product>()
        .HasMany(p => p.Versions)
        .WithOne(v => v.Product)
        .HasForeignKey(v => v.ProductId);

      // Deduplication
      builder.Entity<ProductVersion>()
        .HasIndex(v => new { v.ProductId, v.Version }).IsUnique();
    }
  }
}
