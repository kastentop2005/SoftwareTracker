namespace SoftwareTracker.Domain
{
  public class ProductVersion
  {
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public string Version { get; set; } = null!;
    public string ReleaseDate { get; set; } = null!;
    public string SourceUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
  }
}
