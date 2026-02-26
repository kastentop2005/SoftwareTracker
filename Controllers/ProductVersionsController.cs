using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Model;

namespace SoftwareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVersionsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductVersionsController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/ProductVersions?page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<object>> GetProductVersions(
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20)
        {
          if (page < 1) page = 1;
          if (pageSize < 1) pageSize = 20;
          if (pageSize > 100) pageSize = 100;

          var totalCount = await _context.ProductVersions.CountAsync();
          var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

          var versions = await _context.ProductVersions
            .Include(v => v.Product)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

          return Ok(new
          {
            page,
            pageSize,
            totalCount,
            totalPages,
            data = versions
          });
        }

        // GET: api/ProductVersions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVersion>> GetProductVersion(Guid id)
        {
            var productVersion = await _context.ProductVersions
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (productVersion == null)
            {
                return NotFound();
            }

            return productVersion;
        }

        // GET: api/ProductVersions/product/5?page=1&pageSize=20
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<object>> GetVersionsByProduct(
          Guid productId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20)
        {
          if (page < 1) page = 1;
          if (pageSize < 1) pageSize = 20;
          if (pageSize > 100) pageSize = 100;

          var totalCount = await _context.ProductVersions
            .Where(v => v.ProductId == productId)
            .CountAsync();
          var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

          var versions = await _context.ProductVersions
            .Include(v => v.Product)
            .Where(v => v.ProductId == productId)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

          return Ok(new
          {
            page,
            pageSize,
            totalCount,
            totalPages,
            data = versions
          });
        }
    }
}
