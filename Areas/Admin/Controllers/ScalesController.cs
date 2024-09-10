using GundamStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScalesController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            var scales = _context.Scales?
                            .Where(s => s.IsDeleted == false)
                            .OrderByDescending(s => s.Created_At)
                            .ToList();
            return View();
        }
    }
}