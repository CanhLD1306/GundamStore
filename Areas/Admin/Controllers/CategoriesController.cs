using GundamStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories?
                            .Where(c => c.IsDeleted == false)
                            .OrderByDescending(c => c.Created_At)
                            .ToList();
            return View(categories);
        }
    }
}