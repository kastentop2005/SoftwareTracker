# ASP .NET update puller
## Goal
This is a .NET 10 ASP.NET Core Web API backend service that:
1. Collects software version data from the internet (GitHub API)
2. Saves it into a PostgreSQL database
3. Exposes the data via REST API with pagination
This is a simplified real-world backend system.
## Technology stack
| Category        | Used                  |
| --------------- | --------------------- |
| Language        | C#                    |
| Platform        | .NET 10               |
| Framework       | ASP.NET Core Web API  |
| ORM             | Entity Framework Core |
| Database        | PostgreSQL            |
| Version Control | Git + GitHub          |
## Tracked software
- [x] [qBittorrent](https://github.com/qbittorrent/qBittorrent)
- [x] [Zimbra collaboration](https://github.com/Zimbra/zm-mailbox)
- [x] [OpenEMR](https://github.com/openemr/openemr)
- [x] [Frappe HR](https://github.com/frappe/hrms)
- [x] [AFFiNE Community Edition](https://github.com/toeverything/AFFiNE)
## How to run
```shell
# Install EF Core tools if you haven't already
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to create the database
dotnet ef database update

# Run
dotnet run
```
## Add PostgreSQL connection string
```powershell
dotnet user-secrets set "connectionStrings:SoftwareTracker" "Host=your_host;Port=your_port;Database=db_name;Username=username;Password=password"
```
## Add GitHub token to bypass rate limits (optional)
```shell
# Using user secrets
dotnet user-secrets set "GitHub:Token" "ghp_your_token_here"

# Or using environment variable
export GitHub__Token="ghp_your_token_here"
```
## Trigger the sync
```powershell
# curl
curl -X POST "http://localhost:5227/api/Sync"

# PowerShell
Invoke-RestMethod -Uri "http://localhost:5227/api/Sync" -Method POST
```
## Fetch versions
```powershell
# curl
curl -X GET "http://localhost:5227/api/products/by-name/qBittorrent/versions?page=1&pageSize=20"

# PowerShell
Invoke-RestMethod -Uri "http://localhost:5227/api/products/by-name/qBittorrent/versions?page=1&pageSize=20" -Method GET
```