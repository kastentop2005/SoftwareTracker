namespace SoftwareTracker.Application.Products
{
  public class OpenEMRCollector(IHttpClientFactory httpClientFactory) : GitHubCollector(httpClientFactory)
  {
    public override string ProductName => "OpenEMR";
    public override string SourceUrl => "https://github.com/openemr/openemr";
    protected override string RepoOwner => "openemr";
    protected override string RepoName => "openemr";

    protected override string ApiEndpoint => "tags";
  }
}
