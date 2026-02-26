namespace SoftwareTracker.Application
{
  public interface IVersionCollector
  {
    string ProductName { get; }
    string SourceUrl { get; }
    Task<IReadOnlyCollection<CollectedVersion>> CollectAsync();
  }
}
