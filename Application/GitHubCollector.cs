using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoftwareTracker.Application
{
  public abstract class GitHubCollector : IVersionCollector
  {
    public abstract string ProductName { get; }
    public abstract string SourceUrl { get; }
    protected abstract string RepoOwner { get; }
    protected abstract string RepoName { get; }

    // JSON fields to parse from response
    private record GitHubRelease
    {
      [JsonPropertyName("tag_name")]
      public string TagName { get; set; } = null!;

      [JsonPropertyName("published_at")]
      public DateTime PublishedAt { get; set; }

      [JsonPropertyName("html_url")]
      public string HtmlUrl { get; set; } = null!;
    }

    public async Task<IReadOnlyCollection<CollectedVersion>> CollectAsync()
    {
      using (HttpClient ghClient = new())
      {
        // Define User-Agent header
        ghClient.DefaultRequestHeaders.Add("User-Agent", "SoftwareTracker-App");
        // Specify the base address
        ghClient.BaseAddress = new Uri($"https://api.github.com/");

        try
        {
          var response = await ghClient.GetAsync($"repos/{RepoOwner}/{RepoName}/releases");
          response.EnsureSuccessStatusCode();

          string jsonResponse = await response.Content.ReadAsStringAsync();

          var releases = JsonSerializer.Deserialize<List<GitHubRelease>>(jsonResponse);

          var collectedVersions = new List<CollectedVersion>();

          foreach (var release in releases)
          {
            var dto = new CollectedVersion
            {
              VersionNumber = release.TagName,
              ReleaseDate = release.PublishedAt,
              SourceUrl = release.HtmlUrl
            };
            collectedVersions.Add(dto);
          }
          return collectedVersions;
        }
        catch (Exception) {
          throw;
        }
      }
    }
  }
}
