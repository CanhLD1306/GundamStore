using System.Security.Claims;
using GundamStore.Data;
using GundamStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        

        public CategoriesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }   

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListCategories(string? searchText)
        {

            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }


            var categories = await _context.Categories
                                        .Where(c => c.IsDeleted == false &&
                                            (string.IsNullOrEmpty(searchText) || 
                                            c.Name.ToLower().Contains(searchText) || 
                                            c.Description.ToLower().Contains(searchText)))
                                        .OrderByDescending(c => c.Created_At)
                                        .ToListAsync();

            return PartialView("_ListCategories", categories);
        }

        public IActionResult Create()
        {
            return PartialView("_CreateCategory", new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string description)
        {

            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var category = new Category
            {
                Name = name,
                Description = description,
                Created_At = DateTime.Now,
                Updated_At = DateTime.Now,
                Created_By = userId,
                Updated_By = userId,
                IsDeleted = false
            };

            _context.Add(category);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Edit(int id)
        {

            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView("_EditCategory", category);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingCategory = await _context.Categories.FindAsync(category.Id);

            if (existingCategory == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Updated_At = DateTime.Now;
            existingCategory.Updated_By = userId;

            try
            {
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, message = "Error updating category." });
            }

            return Json(new { success = true });

        }
        
        public async Task<IActionResult> Delete(int id)
        {
            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteCategory", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context == null || _context.Categories == null)
            {
                return Problem("Database context is not available.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            category.Updated_At = DateTime.Now;
            category.Updated_By = userId;
            category.IsDeleted = true;
            _context.Update(category);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}