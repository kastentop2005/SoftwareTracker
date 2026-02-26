using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoftwareTracker.Application
{
  public abstract class GitHubCollector(IHttpClientFactory httpClientFactory) : IVersionCollector
  {
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public abstract string ProductName { get; }
    public abstract string SourceUrl { get; }
    protected abstract string RepoOwner { get; }
    protected abstract string RepoName { get; }
    protected abstract string ApiEndpoint { get; }
    public string Developer => RepoOwner;

    private record GitHubRelease
    {
      [JsonPropertyName("tag_name")]
      public string? TagName { get; set; }

      [JsonPropertyName("name")]
      public string? Name { get; set; }

      [JsonPropertyName("published_at")]
      public DateTime? PublishedAt { get; set; }

      [JsonPropertyName("html_url")]
      public string? HtmlUrl { get; set; }

      [JsonPropertyName("zipball_url")]
      public string? ZipUrl { get; set; }
    }

    public async Task<IReadOnlyCollection<CollectedVersion>> CollectAsync()
    {
      var httpClient = _httpClientFactory.CreateClient("GitHub");

      try
      {
        var response = await httpClient.GetAsync($"repos/{RepoOwner}/{RepoName}/{ApiEndpoint}");
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();
        var releases = JsonSerializer.Deserialize<List<GitHubRelease>>(jsonResponse);

        if (releases == null || releases.Count == 0)
        {
          return [];
        }

        var collectedVersions = new List<CollectedVersion>();

        foreach (var release in releases)
        {
          var dto = new CollectedVersion
          {
            VersionNumber = release.TagName ?? release.Name ?? "Unknown",
            ReleaseDate = release.PublishedAt?.ToString("O") ?? DateTime.MinValue.ToString("O"),
            SourceUrl = release.HtmlUrl ?? release.ZipUrl ?? SourceUrl
          };
          collectedVersions.Add(dto);
        }
        return collectedVersions;
      }
      catch (HttpRequestException ex)
      {
        // Log the error and return empty collection instead of crashing
        Console.WriteLine($"Failed to collect versions for {ProductName}: {ex.Message}");
        return [];
      }
      catch (JsonException ex)
      {
        Console.WriteLine($"Failed to parse response for {ProductName}: {ex.Message}");
        return [];
      }
    }
  }
}