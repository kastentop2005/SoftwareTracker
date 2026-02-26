namespace SoftwareTracker.Application.Products
{
  public class OpenEMRCollector : GitHubCollector
  {
    public override string ProductName => "OpenEMR";
    public override string SourceUrl => "https://github.com/openemr/openemr";
    protected override string RepoOwner => "openemr";
    protected override string RepoName => "openemr";

    protected override string ApiEndpoint => "tags";
  }
}
