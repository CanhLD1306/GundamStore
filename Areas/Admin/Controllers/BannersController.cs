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
    [Area("Admin")]
    // [Authorize(Roles = "Admin")]
    public class BannersController : BaseController
    {
        private readonly IBannerService _bannerService;
        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _bannerService.ListAllBannersAsync();
            return View(banners);

        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile fileImage, string description)
        {
            try
            {
                await _bannerService.CreateBannerAsync(fileImage, description);
                return Json(new Result { Success = true, Message = "Banner created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Banner not found.";
                return RedirectToAction("Index");
            }
            return View(banner);
        }

        public async Task<IActionResult> Edit(long id, IFormFile fileImage, string description)
        {

            if (fileImage == null)
            {
                ModelState.AddModelError("Banner", "Image is required!");
                return View("Create", new Banner { Description = description });
            }

            if (!fileImage.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("The file is invalid. Please upload an image file.");
            }

            var result = await _bannerService.UpdateBannerAsync(id, fileImage, description);

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

        public async Task<IActionResult> Delete(long id)
        {

            var result = await _bannerService.DeleteBannerAsync(id);

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
    }
}