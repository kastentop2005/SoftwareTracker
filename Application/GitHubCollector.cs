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

    // Use abstract endpoint because some repos have tags, while others got proper releases
    protected abstract string ApiEndpoint { get; }

    private record GitHubRelease
    {
      // Tag endpoint uses "name", Release endpoint uses "tag_name"
      [JsonPropertyName("tag_name")]
      public string? TagName { get; set; }

      [JsonPropertyName("name")]
      public string? Name { get; set; }

      // Tag endpoint doesn't have this
      [JsonPropertyName("published_at")]
      public DateTime? PublishedAt { get; set; }

      // Tag endpoint uses "zipball_url", Release endpoint uses "html_url"
      [JsonPropertyName("html_url")]
      public string? HtmlUrl { get; set; }

      [JsonPropertyName("zipball_url")]
      public string? ZipUrl { get; set; }
    }

    public async Task<IReadOnlyCollection<CollectedVersion>> CollectAsync()
    {
      using HttpClient ghClient = new();

      // Define User-Agent header
      ghClient.DefaultRequestHeaders.Add("User-Agent", "SoftwareTracker-App");
      // Specify the base address
      ghClient.BaseAddress = new Uri($"https://api.github.com/");

      try
      {
        var response = await ghClient.GetAsync($"repos/{RepoOwner}/{RepoName}/{ApiEndpoint}");
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();

        var releases = JsonSerializer.Deserialize<List<GitHubRelease>>(jsonResponse);

        var collectedVersions = new List<CollectedVersion>();

        foreach (var release in releases)
        {
          var dto = new CollectedVersion
          {
            // Pick TagName if it exists, otherwise use Name
            VersionNumber = release.TagName ?? release.Name ?? "Unknown",

            // Use the date if found, otherwise use current time or null
            ReleaseDate = release.PublishedAt?.ToString("O") ?? DateTime.MinValue.ToString("O"),

            // Pick the web link, or the zip link, or the base repo URL
            SourceUrl = release.HtmlUrl ?? release.ZipUrl ?? SourceUrl
          };
          collectedVersions.Add(dto);
        }
        return collectedVersions;
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}