using SoftwareTracker.Model;
using Microsoft.EntityFrameworkCore;

namespace SoftwareTracker.Data
{
  public class ProductContext : DbContext
  {
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVersion> ProductVersions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseNpgsql("<connection string>");
    }
  }
}
