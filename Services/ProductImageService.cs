using GudandStore.Interfaces;
using GundamStore.Admin.ViewModels;
using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserService _userService;

        private readonly IFirebaseStorageService _firebaseStorageService;

        private const string ProductImageFolder = "ProductImage";

        public ProductImageService(ApplicationDbContext context, IUserService userService, IFirebaseStorageService firebaseStorageService)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
            _userService = userService ?? throw new InvalidOperationException("UserService is not initialized.");
            _firebaseStorageService = firebaseStorageService ?? throw new InvalidOperationException("FirebaseStorageService is not initialized.");
        }


        public async Task<List<ProductImage>> GetProductImagesByProductIdAsync(long id)
        {
            var productImages = await _context.ProductImages!
                                        .Where(pi => !pi.IsDeleted && pi.ProductId == id)
                                        .ToListAsync();

            return productImages;
        }

        /// <summary>
        public async Task<ProductImage> GetImageDefaultAsync(long id)
        {
            var productImage = await _context.ProductImages!
                                            .Where(pi => !pi.IsDeleted && pi.ProductId == id && pi.IsDefault)
                                            .FirstOrDefaultAsync();

            if (productImage == null)
            {
                throw new KeyNotFoundException("Product image not found.");
            }

            return productImage;
        }
        public async Task<ProductImage> GetProductImageByIdAsync(long id)
        {
            var productImage = await _context.ProductImages!
                                            .Where(pi => !pi.IsDeleted && pi.Id == id)
                                            .FirstOrDefaultAsync();

            if (productImage == null)
            {
                throw new KeyNotFoundException("Product image not found.");
            }

            return productImage;
        }

        public async Task<long> InsertImagesAsync(long id, ImageViewModel model)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Product id cannot be null.");
            }

            if (model.File == null)
            {
                throw new ArgumentException("Image cannot be empty.");
            }
            try
            {

                var imageUrl = await _firebaseStorageService.UploadFileAsync(model.File, ProductImageFolder);
                var productImage = new ProductImage
                {
                    ProductId = id,
                    ImageURL = imageUrl,
                    IsDefault = model.IsDefault,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = await _userService.GetUserId(),
                    UpdatedBy = await _userService.GetUserId(),
                    IsDeleted = model.IsDeleted
                };
                await _context.ProductImages!.AddAsync(productImage);
                await _context.SaveChangesAsync();
                return productImage.Id;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the product images.");
            }
        }

        public async Task<bool> DeleteImagesAsync(long id)
        {
            var image = await GetProductImageByIdAsync(id);

            try
            {
                image.IsDeleted = true;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = await _userService.GetUserId();

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while deleting the product images.");
            }
        }

        public async Task<bool> UpdateImageAsync(ImageViewModel model)
        {
            var image = await GetProductImageByIdAsync(model.Id);

            try
            {
                image.IsDefault = model.IsDefault;
                image.IsDeleted = model.IsDeleted;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = await _userService.GetUserId();

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while updating the product images.");
            }
        }
    }
}