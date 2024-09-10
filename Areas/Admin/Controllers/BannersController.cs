using System.Security.Claims;
using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BannersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public BannersController(ApplicationDbContext Context, IFirebaseStorageService firebaseStorageService)
        {
            _context = Context;
            _firebaseStorageService = firebaseStorageService;
        }

        public IActionResult Index()
        {
            var banners = _context?.Banners?
                            .Where(b => b.IsDeleted == false)
                            .OrderByDescending(b => b.Created_At)
                            .ToList();
            return View(banners);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBanner(Banner model, IFormFile fileImage)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;
                if (fileImage != null && fileImage.Length > 0)
                {
                    imageUrl = await _firebaseStorageService.UploadFileAsync(fileImage, "banners");
                }

                model.FileImage = imageUrl;
                model.Created_At = DateTime.UtcNow;
                model.Updated_At = DateTime.UtcNow;
                // model.Created_By = GetUserId();
                // model.Updated_By = GetUserId();
                model.IsDeleted = false;

                _context?.Banners?.Add(model);
                if (_context != null)
                {
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

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