using System.Security.Claims;
using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BannersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFirebaseStorageService _firebaseStorageService;

        private readonly ILogger<BannersController> _logger;

        public BannersController(ApplicationDbContext Context, IFirebaseStorageService firebaseStorageService, ILogger<BannersController> logger)
        {
            _context = Context;
            _firebaseStorageService = firebaseStorageService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (_context == null || _context.Banners == null)
            {
                return Problem("Database context is not available.");
            }
            var banners = await _context.Banners
                            .Where(b => b.IsDeleted == false)
                            .OrderByDescending(b => b.Created_At)
                            .ToListAsync();
            return View(banners);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateBanner(IFormFile fileImage, string description)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;
                
                // Kiểm tra và upload file nếu có
                if (fileImage != null && fileImage.Length > 0)
                {
                    try
                    {
                        imageUrl = await _firebaseStorageService.UploadFileAsync(fileImage, "banners");
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nếu việc upload thất bại
                        _logger.LogError("Error uploading image to Firebase: " + ex.Message);
                        ModelState.AddModelError(string.Empty, "There was an error uploading the image. Please try again.");
                        return View("Index"); // Trả về view cùng với lỗi
                    }
                }
                var banner = new Banner
                {
                    // Thiết lập các giá trị khác cho banner
                    FileImage = imageUrl,
                    Created_At = DateTime.UtcNow,
                    Updated_At = DateTime.UtcNow,
                    Created_By = GetUserId(),
                    Updated_By = GetUserId(),
                    IsDeleted = false
                };


                // Lưu vào database
                try
                {
                    _context?.Banners?.Add(banner);
                    if (_context != null)
                    {
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Banner created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu xảy ra lỗi khi lưu vào database
                    _logger.LogError("Error saving banner to the database: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the banner. Please try again.");
                }
            }

            // Nếu có lỗi, trả về lại view cùng với ModelState
            TempData["ErrorMessage"] = "There was an error creating the banner.";
            return View("Index");
        }


        private string GetUserId()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
    }
}