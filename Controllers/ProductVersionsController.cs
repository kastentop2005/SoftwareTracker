using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        // GET: api/ProductVersions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVersion>>> GetProductVersions()
        {
            return await _context.ProductVersions.ToListAsync();
        }

        // GET: api/ProductVersions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVersion>> GetProductVersion(Guid id)
        {
            var productVersion = await _context.ProductVersions.FindAsync(id);

            if (productVersion == null)
            {
                return NotFound();
            }

            return productVersion;
        }

        // PUT: api/ProductVersions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductVersion(Guid id, ProductVersion productVersion)
        {
            if (id != productVersion.Id)
            {
                return BadRequest();
            }

            _context.Entry(productVersion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductVersionExists(id))
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

        // POST: api/ProductVersions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductVersion>> PostProductVersion(ProductVersion productVersion)
        {
            _context.ProductVersions.Add(productVersion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVersion", new { id = productVersion.Id }, productVersion);
        }

        // DELETE: api/ProductVersions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVersion(Guid id)
        {
            var productVersion = await _context.ProductVersions.FindAsync(id);
            if (productVersion == null)
            {
                return NotFound();
            }

            _context.ProductVersions.Remove(productVersion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductVersionExists(Guid id)
        {
            return _context.ProductVersions.Any(e => e.Id == id);
        }
    }
}
