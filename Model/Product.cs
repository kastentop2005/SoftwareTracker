namespace SoftwareTracker.Model
{
  public class Product
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Developer { get; set; } = null!;
    public string SourceUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<ProductVersion> Versions { get; set; } = [];
  }
}