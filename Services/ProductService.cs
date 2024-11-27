using GudandStore.Interfaces;
using GundamStore.Admin.ViewModels;
using GundamStore.Areas.Admin.ViewModels;
using GundamStore.Data;
using GundamStore.Dto;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace GundamStore.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserService _userService;

        private readonly IProductImageService _productImageService;

        public ProductService(ApplicationDbContext context, IUserService userService, IProductImageService productImageService)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
            _userService = userService ?? throw new InvalidOperationException("UserService is not initialized.");
            _productImageService = productImageService ?? throw new InvalidOperationException("ProductImageService is not initialized.");
        }

        public async Task<List<ProductWithImageDto>> GetAllProductsAsync()
        {
            var productsWithImage = await _context.Products!
                                    .Where(p => !p.IsDeleted)
                                    .OrderByDescending(p => p.CreatedAt)
                                    .Select(p => new ProductWithImageDto
                                    {
                                        Id = p.Id,
                                        Name = p.Name,
                                        Brand = p.Brand,
                                        CategoryId = p.CategoryId,
                                        ScaleId = p.ScaleId,
                                        Price = p.Price,
                                        StockQuantity = p.StockQuantity,
                                        IsActive = p.IsActive,
                                        IsDeleted = p.IsDeleted,
                                        ImageDefault = _context.ProductImages!
                                                        .Where(pi => pi.IsDefault && !pi.IsDeleted && pi.ProductId == p.Id)
                                                        .Select(pi => pi.ImageURL)
                                                        .FirstOrDefault()
                                    })
                                    .ToListAsync();

            return productsWithImage;
        }

        public async Task<Product> GetProductByIdAsync(long id)
        {
            var product = await _context.Products!
                                        .Where(p => !p.IsDeleted && p.Id == id)
                                        .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return product;
        }



        public async Task<long> InsertProductAsync(ProductViewModel model, List<ImageViewModel> images)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Product model cannot be null.");
            }

            if (images == null || !images.Any())
            {
                throw new ArgumentException("At least one image is required for the product.", nameof(images));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var product = new Product
                {
                    Name = model.Name,
                    Brand = model.Brand,
                    CategoryId = model.CategoryId,
                    ScaleId = model.ScaleId,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = await _userService.GetUserId(),
                    UpdatedBy = await _userService.GetUserId(),
                    IsDeleted = false
                };

                await _context.Products!.AddAsync(product);
                await _context.SaveChangesAsync();

                foreach (var image in images)
                {
                    await _productImageService.InsertImagesAsync(product.Id, image);
                }
                await transaction.CommitAsync();
                return product.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> UpdateProductAsync(ProductViewModel model, List<ImageViewModel> images)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var product = await GetProductByIdAsync(model.Id);

            try
            {
                product.Name = model.Name;
                product.Brand = model.Brand;
                product.CategoryId = model.CategoryId;
                product.ScaleId = model.ScaleId;
                product.Price = model.Price;
                product.StockQuantity = model.StockQuantity;
                product.Description = model.Description;
                product.IsActive = model.IsActive;
                product.UpdatedAt = DateTime.UtcNow;
                product.UpdatedBy = await _userService.GetUserId();

                await _context.SaveChangesAsync();

                foreach (var image in images)
                {
                    if (image.Id == 0)
                    {
                        await _productImageService.InsertImagesAsync(product.Id, image);
                    }
                    else
                    {
                        await _productImageService.UpdateImageAsync(image);
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("An error occurred while updating the product.");
            }
        }

        public async Task<bool> DeleteProductAsync(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var product = await GetProductByIdAsync(id);

            try
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
                product.UpdatedBy = await _userService.GetUserId();

                await _context.SaveChangesAsync();

                var images = await _productImageService.GetProductImagesByProductIdAsync(product.Id);
                foreach (var image in images)
                {
                    await _productImageService.DeleteImagesAsync(image.Id);
                }
                await transaction.CommitAsync();
                return true;

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("An error occurred while deleting the product.");
            }
        }

        public Task<IPagedList<ProductWithImageDto>> GetProductsAsync(string searchString, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}