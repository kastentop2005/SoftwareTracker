using Microsoft.AspNetCore.Mvc;
using SoftwareTracker.Application;

namespace SoftwareTracker.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SyncController : ControllerBase
  {
    private readonly VersionSyncService _syncService;

    public SyncController(VersionSyncService syncService)
    {
      _syncService = syncService;
    }

    // POST: api/Sync
    [HttpPost]
    public async Task<IActionResult> TriggerSync()
    {
      await _syncService.SyncAllAsync();
      return Ok(new 
      { 
        message = "Sync completed"
      });
    }
  }
}