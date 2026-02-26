using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Model;

namespace SoftwareTracker.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController(ProductContext context) : ControllerBase
  {
    private readonly ProductContext _context = context;

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetProducts()
    {
      var products = await _context.Products
        .OrderBy(p => p.Name)
        .Select(p => new
        {
          p.Id,
          p.Name,
          p.Developer,
          p.SourceUrl,
          VersionCount = p.Versions.Count
        })
        .ToListAsync();

      return Ok(products);
    }

    // GET: api/products/{productId}/versions?page=1&pageSize=20
    [HttpGet("{productId}/versions")]
    public async Task<ActionResult<object>> GetProductVersions(
      Guid productId,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 20)
    {
      if (page < 1) page = 1;
      if (pageSize < 1) pageSize = 20;
      if (pageSize > 100) pageSize = 100;

      // Check if product exists
      var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
      if (!productExists)
      {
        return NotFound(new { message = "Product not found" });
      }

      var total = await _context.ProductVersions
        .Where(v => v.ProductId == productId)
        .CountAsync();

      var items = await _context.ProductVersions
        .Where(v => v.ProductId == productId)
        .OrderByDescending(v => v.ReleaseDate) // Sort by release date, newest first
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(v => new
        {
          version = v.Version,
          releaseDate = v.ReleaseDate,
          sourceUrl = v.SourceUrl
        })
        .ToListAsync();

      return Ok(new
      {
        total,
        page,
        pageSize,
        items
      });
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
      var product = await _context.Products
        .Include(p => p.Versions)
        .FirstOrDefaultAsync(p => p.Id == id);

      if (product == null)
      {
        return NotFound();
      }

      return product;
    }
  }
}
