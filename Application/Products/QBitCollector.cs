namespace SoftwareTracker.Application.Products
{
  public class QBitCollector : GitHubCollector
  {
    public override string ProductName => "qBittorrent";
    public override string SourceUrl => "https://github.com/qbittorrent/qBittorrent";
    protected override string RepoOwner => "qbittorrent";
    protected override string RepoName => "qBittorrent";

    protected override string ApiEndpoint => "tags";
  }
}
