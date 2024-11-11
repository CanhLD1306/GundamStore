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
            CheckCategoriesInitialized();
            return await (_context.Categories!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync() ?? Task.FromResult(new List<Category>()));
        }

        public async Task<List<Category>> ListAllCategoryAsync(int top)
        {
            CheckCategoriesInitialized();
            return await (_context.Categories!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .Take(top)
                            .ToListAsync() ?? Task.FromResult(new List<Category>()));
        }

        public async Task<IPagedList<Category>> ListAllAsync(string searchString, int page, int pageSize)
        {
            CheckCategoriesInitialized();

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
            CheckCategoriesInitialized();

            return await _context.Categories!.AnyAsync(c => !c.IsDeleted
                                                        && c.Name != null
                                                        && c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<long> InsertAsync(Category category)
        {
            CheckCategoriesInitialized();

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
            CheckCategoriesInitialized();

            var category = await _context.Categories!.FindAsync(id);

            if (category == null || category.IsDeleted)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        public async Task<Category> ViewDetailAsync(long id)
        {
            CheckCategoriesInitialized();

            var category = await _context.Categories!.FindAsync(id);

            if (category == null || category.IsDeleted)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        public async Task<bool> UpdateAsync(Category model)
        {
            try
            {
                CheckCategoriesInitialized();

                var category = await _context.Categories!.FindAsync(model.Id);

                if (category == null || category.IsDeleted)
                {
                    return false;
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.UpdatedAt = model.UpdatedAt;
                category.UpdatedBy = model.UpdatedBy;
                category.IsDeleted = model.IsDeleted;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckCategoriesInitialized()
        {
            if (_context?.Categories == null)
            {
                throw new InvalidOperationException("Categories DbSet is not initialized.");
            }
        }
    }
}