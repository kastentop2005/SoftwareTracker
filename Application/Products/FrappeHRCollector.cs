namespace SoftwareTracker.Application.Products
{
  public class FrappeHRCollector(IHttpClientFactory httpClientFactory) : GitHubCollector(httpClientFactory)
  {
    public override string ProductName => "hrms";
    public override string SourceUrl => "https://github.com/frappe/hrms";
    protected override string RepoOwner => "frappe";
    protected override string RepoName => "hrms";

    protected override string ApiEndpoint => "tags";
  }
}
