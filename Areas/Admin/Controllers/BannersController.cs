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
    public class BannersController : BaseController
    {
        private readonly IBannerService _bannerService;
        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListAllBanners()
        {
            var banners = await _bannerService.ListAllBannersAsync();
            return PartialView("_ListBanners", banners);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile fileImage, string description)
        {
            try
            {
                await _bannerService.CreateBannerAsync(fileImage, description);
                return Json(new Result { Success = true, Message = "Banner created successfully." });
            }
            catch (ArgumentException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var banner = await _bannerService.GetBannerByIdAsync(id);
                return PartialView("_EditBannerModal", banner);
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, string description)
        {
            try
            {
                await _bannerService.UpdateBannerAsync(id, description);
                return Json(new Result { Success = true, Message = "Banner updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var banner = await _bannerService.GetBannerByIdAsync(id);
                return PartialView("_DeleteBannerModal", banner);
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(long id)
        {
            try
            {
                await _bannerService.DeleteBannerAsync(id);
                return Json(new Result { Success = true, Message = "Banner delete successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

    }
}