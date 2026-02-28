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

      [JsonPropertyName("commit")]
      public GitHubCommitRef? Commit { get; set; }
    }

    private record GitHubCommitRef
    {
      [JsonPropertyName("sha")]
      public string? Sha { get; set; }

      [JsonPropertyName("url")]
      public string? Url { get; set; }
    }

    private record GitHubCommitDetail
    {
      [JsonPropertyName("commit")]
      public GitHubCommitInfo? Commit { get; set; }
    }

    private record GitHubCommitInfo
    {
      [JsonPropertyName("author")]
      public GitHubAuthor? Author { get; set; }

      [JsonPropertyName("committer")]
      public GitHubAuthor? Committer { get; set; }
    }

    private record GitHubAuthor
    {
      [JsonPropertyName("date")]
      public DateTime? Date { get; set; }
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
          DateTime? releaseDate = release.PublishedAt;

          // If PublishedAt is null (tags endpoint), fetch commit date
          if (releaseDate == null && release.Commit?.Url != null)
          {
            releaseDate = await GetCommitDateAsync(httpClient, release.Commit.Url);
          }

          var dto = new CollectedVersion
          {
            VersionNumber = release.TagName ?? release.Name ?? "Unknown",
            ReleaseDate = releaseDate?.ToString("O") ?? string.Empty,
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

    private async Task<DateTime?> GetCommitDateAsync(HttpClient httpClient, string commitUrl)
    {
      try
      {
        var response = await httpClient.GetAsync(commitUrl);
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();
        var commitDetail = JsonSerializer.Deserialize<GitHubCommitDetail>(jsonResponse);

        // Prefer committer date (when it was added to the repo) over author date
        return commitDetail?.Commit?.Committer?.Date ?? commitDetail?.Commit?.Author?.Date;
      }
      catch
      {
        // If we can't fetch commit date, return null
        return null;
      }
    }
  }
}