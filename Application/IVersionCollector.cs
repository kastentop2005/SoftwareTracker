namespace SoftwareTracker.Application
{
  public interface IVersionCollector
  {
    string ProductName { get; }
    string SourceUrl { get; }
    string Developer { get; }
    Task<IReadOnlyCollection<CollectedVersion>> CollectAsync();
  }
}
