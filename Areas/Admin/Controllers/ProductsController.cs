using GundamStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GundamStore.Areas.Admin.Controllers
{
    // [Area("Admin")]
    // [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            // var products = _context.Products?
            //                 .Where(p => p.IsDeleted == false)
            //                 .OrderByDescending(p => p.Created_At)
            //                 .ToList();
            return View();
        }
    }
}