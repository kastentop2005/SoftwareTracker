using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Model;

namespace SoftwareTracker.Application
{
  public record CollectedVersion
  {
    public string VersionNumber { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
  }

  public class VersionSyncService(IEnumerable<IVersionCollector> collectors, ProductContext context)
  {
    private readonly IEnumerable<IVersionCollector> _collectors = collectors;
    private readonly ProductContext _context = context;

    public async Task SyncAllAsync()
    {
      foreach (var collector in _collectors)
      {
        // Collect from sources
        var newVersions = await collector.CollectAsync();

        // Find the corresponding product in the database
        var product = await _context.Products
          .Include(p => p.Versions)
          .FirstOrDefaultAsync(p => p.SourceUrl == collector.SourceUrl);

        // If program doesn't exist, create it
        if (product == null)
        {
          product = new Product
          {
            Id = Guid.NewGuid(),
            Name = collector.ProductName,
            Developer = "Unknown",
            SourceUrl = collector.SourceUrl,
            CreatedAt = DateTime.UtcNow,
          };
          _context.Products.Add(product);
        }

        // Filter out existing versions
        var existingVersions = product.Versions
          .Select(v => v.Version)
          .ToHashSet();

        var versionsToAdd = newVersions
          .Where(v => !existingVersions.Contains(v.VersionNumber))
          .ToList();

        // Map DTO to Entity
        foreach (var collectedVersion in versionsToAdd) 
        {
          var productVersion = new ProductVersion
          {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Version = collectedVersion.VersionNumber,
            ReleaseDate = collectedVersion.ReleaseDate,
            SourceUrl = collectedVersion.SourceUrl,
            CreatedAt = DateTime.UtcNow
          };
          product.Versions.Add(productVersion);
        }
      }

      // Commit changes to the database
      await _context.SaveChangesAsync();
    }
  };
}
