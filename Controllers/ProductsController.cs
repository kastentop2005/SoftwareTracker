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

    // GET: api/Products?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<object>> GetProducts(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10)
    {
      if (page < 1) page = 1;
      if (pageSize < 1) pageSize = 10;
      if (pageSize > 100) pageSize = 100; // Max 100 items per page

      var totalCount = await _context.Products.CountAsync();
      var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

      var products = await _context.Products
        .Include(p => p.Versions)
        .OrderBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

      return Ok(new
      {
        page,
        pageSize,
        totalCount,
        totalPages,
        data = products
      });
    }

    // GET: api/Products/5
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

    // PUT: api/Products/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(Guid id, Product product)
    {
      if (id != product.Id)
      {
        return BadRequest();
      }

      _context.Entry(product).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ProductExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Products
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      _context.Products.Remove(product);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ProductExists(Guid id)
    {
      return _context.Products.Any(e => e.Id == id);
    }
  }
}
