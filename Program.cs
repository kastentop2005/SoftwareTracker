using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Define secret connection string
builder.Services.AddDbContext<ProductContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("SoftwareTracker")));

var app = builder.Build();