using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanayii.Core.Entities;
using Sanayii.Core.Repositories;
using Sanayii.Repository.Data;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categories;

        public CategoriesController(IGenericRepository<Category> categories)
        {
            _categories = categories;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Category>>> GetCategories()
        {
            var categories = await _categories.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _categories.GetByIdAsync(id);

            if (category is null)
            {
                return NotFound();
            }

            return category;
        }

        // GET : api/Categories/{name}
        //[HttpGet]
        //public async Task<ActionResult<Category>> GetCategoryByName([FromQuery] string name)
        //{
        //    //var category = await _categories.GetByNameAsync(name);

        //    if (category is null)
        //        return NotFound();
        //    else
        //        return Ok(category);
        //}

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingCategory = _categories.GetByIdAsync(id);
            if (existingCategory is null)
            {
                return NotFound("Category not found");
            }

            _categories.Update(category);
            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            await _categories.Add(category);

            //await _categories.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _categories.Delete(category);
            //await _categories.SaveChangesAsync();

            return NoContent();
        }

        //private bool CategoryExists(int id)
        //{
        //    return _context.Categories.Any(e => e.Id == id);
        //}
    }
}