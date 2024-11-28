using GundamStore.Interfaces;
using GundamStore.Models;
using GundamStore.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;


namespace GundamStore.Controllers
{

    public class HomeController : BaseController
    {

        private readonly IBannerService _bannerService;

        public HomeController(IBannerService bannerService)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListAllBanners()
        {
            var banners = await _bannerService.GetAllBannersAsync();
            return PartialView("_ListBanners", banners);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
