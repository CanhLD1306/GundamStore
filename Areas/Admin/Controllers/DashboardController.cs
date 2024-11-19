using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin")]
    public class DashboardController : BaseController

    {
        public IActionResult Index()
        {
            return View();
        }
    }
}