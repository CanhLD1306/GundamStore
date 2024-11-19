using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Services
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;

        private readonly IFirebaseStorageService _firebaseStorageService;

        private readonly IUserService _userService;

        private const string BannerFolder = "Banner";

        public BannerService(ApplicationDbContext context, IUserService userService, IFirebaseStorageService firebaseStorageService)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
            _userService = userService ?? throw new InvalidOperationException("UserService is not initialized.");
            _firebaseStorageService = firebaseStorageService ?? throw new InvalidOperationException("FirebaseStorageService is not initialized.");
        }

        public async Task<List<Banner>> ListAllBannersAsync()
        {
            return await _context.Banners!
                            .Where(b => !b.IsDeleted)
                            .OrderByDescending(b => b.CreatedAt)
                            .ToListAsync();
        }

        public async Task<Banner> GetBannerByIdAsync(long id)
        {

            var banner = await _context.Banners!.Where(b => !b.IsDeleted && b.Id == id)
                                                    .FirstOrDefaultAsync();

            if (banner == null)
            {
                throw new KeyNotFoundException($"Banner with ID {id} not found.");
            }

            return banner;
        }

        public async Task<long> CreateBannerAsync(IFormFile fileImage, string description)
        {
            var banners = await ListAllBannersAsync();

            if (banners.Count >= 5)
            {
                throw new InvalidOperationException("Cannot add more banners. Maximum of 5 banners allowed.");
            }

            if (fileImage == null)
            {
                throw new ArgumentException("File image is required.", nameof(fileImage));
            }

            if (!fileImage.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("The file is invalid. Please upload an image file.");
            }

            var imageUrl = await _firebaseStorageService.UploadFileAsync(fileImage, BannerFolder);

            var banner = new Banner
            {
                ImageURL = imageUrl,
                Description = description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = await _userService.GetUserId(),
                UpdatedBy = await _userService.GetUserId(),
                IsDeleted = false
            };

            await _context.Banners!.AddAsync(banner);
            await _context.SaveChangesAsync();
            return banner.Id;

        }

        public async Task<bool> UpdateBannerAsync(long id, IFormFile fileImage, string description)
        {
            var banner = await _context.Banners!.Where(b => !b.IsDeleted && b.Id == id)
                                                    .FirstOrDefaultAsync();

            if (banner == null)
            {
                throw new KeyNotFoundException($"Banner with ID {id} not found.");
            }

            if (fileImage == null)
            {
                banner.Description = description;
            }
            else
            {
                if (!fileImage.ContentType.StartsWith("image/"))
                {
                    throw new ArgumentException("The file is invalid. Please upload an image file.");
                }
                var imageUrl = await _firebaseStorageService.UploadFileAsync(fileImage, BannerFolder);
                banner.ImageURL = imageUrl;
            }
            banner.UpdatedAt = DateTime.Now;
            banner.UpdatedBy = await _userService.GetUserId();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBannerAsync(long id)
        {
            var banner = await _context.Banners!.Where(b => !b.IsDeleted && b.Id == id)
                                                    .FirstOrDefaultAsync();

            if (banner == null)
            {
                throw new KeyNotFoundException($"Banner with ID {id} not found.");
            }


            banner.UpdatedAt = DateTime.Now;
            banner.UpdatedBy = await _userService.GetUserId();
            banner.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}