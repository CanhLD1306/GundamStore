using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
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

        public async Task<List<Banner>> GetAllBannersAsync()
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
                throw new KeyNotFoundException("Banner not found.");
            }

            return banner;
        }

        public async Task<long> InsertBannerAsync(IFormFile fileImage, string description)
        {
            var banners = await GetAllBannersAsync();

            if (banners.Count >= 5)
            {
                throw new InvalidOperationException("Cannot add more banners. Maximum of 5 banners allowed.");
            }

            if (fileImage == null)
            {
                throw new ArgumentException("File image is required.");
            }

            if (!fileImage.ContentType.StartsWith("image/"))
            {
                throw new InvalidOperationException("The file is invalid. Please upload an image file.");
            }

            try
            {
                var imageUrl = await _firebaseStorageService.UploadFileAsync(fileImage, BannerFolder);

                var banner = new Banner
                {
                    ImageURL = imageUrl,
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = await _userService.GetUserId(),
                    UpdatedBy = await _userService.GetUserId(),
                    IsDeleted = false
                };

                await _context.Banners!.AddAsync(banner);
                await _context.SaveChangesAsync();
                return banner.Id;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the banner.");
            }

        }

        public async Task<bool> UpdateBannerAsync(long id, string description)
        {
            var banner = await GetBannerByIdAsync(id);

            try
            {
                banner.Description = description;
                banner.UpdatedAt = DateTime.UtcNow;
                banner.UpdatedBy = await _userService.GetUserId();
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while editing the banner.");
            }
        }

        public async Task<bool> DeleteBannerAsync(long id)
        {
            var banner = await GetBannerByIdAsync(id);

            try
            {
                banner.UpdatedAt = DateTime.UtcNow;
                banner.UpdatedBy = await _userService.GetUserId();
                banner.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while deleting the banner.");
            }
        }
    }

}