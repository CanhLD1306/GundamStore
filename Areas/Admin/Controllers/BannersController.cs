using System.Security.Claims;
using GundamStore.Common;
using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Areas.Admin.Controllers
{
    // [Area("Admin")]
    // [Authorize(Roles = "Admin")]
    public class BannersController : BaseController
    {
        private readonly IBannerService _bannerService;
        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
        }

        public async Task<ActionResult> Index()
        {
            var banners = await _bannerService.ListAllBannersAsync();
            return View(banners);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(IFormFile fileImage, string description)
        {
            var adminSession = GetAdminSession();
            if (adminSession == null || adminSession.UserId == null)
            {
                TempData["SessionError"] = "Session is not valid or has expired. Please log in again.";
                return RedirectToAction("Index");
            }

            if (fileImage == null)
            {
                ModelState.AddModelError("Banner", "Image is required!");
                return View("Create", new Banner { Description = description });
            }

            var result = await _bannerService.CreateBannerAsync(fileImage, description, adminSession.UserId);

            if (result > 0)
            {
                TempData["bannerSuccess"] = "Banner created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("banner", "Failed to create banner.");
            }
            return View("Create");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Banner not found.";
                return RedirectToAction("Index");
            }
            return View(banner);
        }

        public async Task<ActionResult> Edit(long id, IFormFile fileImage, string description)
        {
            var adminSession = GetAdminSession();
            if (adminSession == null || adminSession.UserId == null)
            {
                TempData["SessionError"] = "Session is not valid or has expired. Please log in again.";
                return RedirectToAction("Index");
            }

            if (fileImage == null)
            {
                ModelState.AddModelError("Banner", "Image is required!");
                return View("Create", new Banner { Description = description });
            }

            var result = await _bannerService.UpdateBannerAsync(id, fileImage, description, adminSession.UserId);

            if (result)
            {
                TempData["bannerSuccess"] = "Banner updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("banner", "Failed to update banner.");
            }

            return View("Edit");
        }

        public async Task<ActionResult> Delete(long id)
        {
            var adminSession = GetAdminSession();
            if (adminSession == null || adminSession.UserId == null)
            {
                TempData["SessionError"] = "Session is not valid or has expired. Please log in again.";
                return RedirectToAction("Index");
            }

            var result = await _bannerService.DeleteBannerAsync(id, adminSession.UserId);

            if (result)
            {
                TempData["bannerSuccess"] = "Banner deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("banner", "Failed to delete banner.");
            }
            return View("Index");
        }

        private AdminLogin? GetAdminSession()
        {
            return HttpContext.Session.GetObjectFromJson<AdminLogin>(Constant.ADMIN_SESSION);
        }
    }
}