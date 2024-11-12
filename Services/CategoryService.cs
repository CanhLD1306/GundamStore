using GundamStore.Data;
using GundamStore.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;


namespace GundamStore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
        }

        public async Task<List<Category>> ListAllAsync()
        {
            return await (_context.Categories!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync() ?? Task.FromResult(new List<Category>()));
        }

        public async Task<List<Category>> ListAllCategoryAsync(int top)
        {
            return await (_context.Categories!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .Take(top)
                            .ToListAsync() ?? Task.FromResult(new List<Category>()));
        }

        public async Task<IPagedList<Category>> ListAllAsync(string searchString, int page, int pageSize)
        {

            IQueryable<Category> category = _context.Categories!.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                category = category.Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchString));
            }

            var result = await category.OrderByDescending(c => c.CreatedAt)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            return new PagedList<Category>(result, page, pageSize);
        }

        public async Task<bool> CheckCategoryAsync(string categoryName)
        {

            return await _context.Categories!.AnyAsync(c => !c.IsDeleted
                                                        && c.Name != null
                                                        && c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<long> CreateCategoryAsync(Category category)
        {

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            await _context.Categories!.AddAsync(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }
        public async Task<Category> GetCategoryByIdAsync(long id)
        {

            var category = await _context.Categories!.Where(c => !c.IsDeleted && c.Id == id)
                                                    .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {

            var existingCategory = await _context.Categories!.Where(c => !c.IsDeleted && c.Id == category.Id)
                                                                .FirstOrDefaultAsync();

            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Banner with ID {category.Id} not found.");
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.UpdatedAt = category.UpdatedAt;
            existingCategory.UpdatedBy = category.UpdatedBy;
            existingCategory.IsDeleted = category.IsDeleted;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Category> ViewDetailAsync(long id)
        {
            var category = await _context.Categories!.Where(c => !c.IsDeleted && c.Id == id)
                                                    .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

    }
}