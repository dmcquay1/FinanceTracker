using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.DataModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinanceTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly FinanceTrackerAPIContext _context;

        public CategoriesController(FinanceTrackerAPIContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (Category.cache == null || Category.cache.Count() < _context.Categories.Count())
            {
               var cats = await GetCategories();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Category.cache = null;
            return NoContent();
        }
  
        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            if (Category.cache == null || Category.cache.Count() < _context.Categories.Count())
            {
                var cats = await GetCategories();
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            try
            {
                if(_context.Categories.Any(e => e.ParentCategoryId == id))
                {
                    return BadRequest("Cannot delete category with child categories.");
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelStateDictionary modelState = new ModelStateDictionary();
                modelState.AddModelError("SQL Exception", ex.InnerException?.Message ?? "Unknown SQL error");
                return new BadRequestObjectResult(modelState);
            }
            Category.cache = null;

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        private bool CategoryIsParent(int id)
        {
            return _context.Categories.Any(e => e.ParentCategoryId == id);
        }
    }
}
