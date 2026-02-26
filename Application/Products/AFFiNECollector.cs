namespace SoftwareTracker.Application.Products
{
  public class AFFiNECollector(IHttpClientFactory httpClientFactory) : GitHubCollector(httpClientFactory)
  {
    public override string ProductName => "AFFiNE";
    public override string SourceUrl => "https://github.com/toeverything/AFFiNE";
    protected override string RepoOwner => "toeverything";
    protected override string RepoName => "AFFiNE";
    protected override string ApiEndpoint => "releases";
  }
}
