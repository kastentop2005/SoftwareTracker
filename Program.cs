using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Application;
using SoftwareTracker.Application.Products;
using SoftwareTracker.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure PostgreSQL connection from user secrets
builder.Services.AddDbContext<ProductContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("SoftwareTracker")));

// Configure HttpClient for GitHub API with authentication
var githubToken = builder.Configuration["GitHub:Token"];
builder.Services.AddHttpClient("GitHub", client =>
{
  client.BaseAddress = new Uri("https://api.github.com/");
  client.DefaultRequestHeaders.Add("User-Agent", "SoftwareTracker-App");
  client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
  client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
  
  // Add authentication if token is provided
  if (!string.IsNullOrWhiteSpace(githubToken))
  {
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {githubToken}");
  }
  
  client.Timeout = TimeSpan.FromSeconds(30);
});

// Register collectors
builder.Services.AddScoped<IVersionCollector, AFFiNECollector>();
builder.Services.AddScoped<IVersionCollector, ZimbraCollector>();
builder.Services.AddScoped<IVersionCollector, QBitCollector>();
builder.Services.AddScoped<IVersionCollector, OpenEMRCollector>();
builder.Services.AddScoped<IVersionCollector, FrappeHRCollector>();

// Register the sync service
builder.Services.AddScoped<VersionSyncService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();