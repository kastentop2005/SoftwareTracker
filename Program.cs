using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Application;
using SoftwareTracker.Application.Products;
using SoftwareTracker.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ProductContext>(options => 
  options.UseNpgsql(builder.Configuration.GetConnectionString("SoftwareTracker")));

// Register collcetors
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