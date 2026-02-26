namespace SoftwareTracker.Application.Products
{
  public class ZimbraCollector : GitHubCollector
  {
    public override string ProductName => "Zimbra";
    public override string SourceUrl => "https://github.com/Zimbra/zm-mailbox";
    protected override string RepoOwner => "Zimbra";
    protected override string RepoName => "zm-mailbox";

    protected override string ApiEndpoint => "tags";
  }
}
