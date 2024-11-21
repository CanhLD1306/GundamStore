using GundamStore.Data;
using GundamStore.Models;
using GundamStore.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;


namespace GundamStore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserService _userService;

        public CategoryService(ApplicationDbContext context, IUserService userService)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
            _userService = userService ?? throw new InvalidOperationException("UserService is not initialized.");
        }

        public async Task<List<Category>> ListAllCategoriesAsync()
        {
            return await _context.Categories!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync();
        }

        public async Task<IPagedList<Category>> ListAllCategoriesAsync(string searchString, int page, int pageSize)
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

        public async Task<Category> GetCategoryByIdAsync(long id)
        {
            var category = await _context.Categories!.Where(c => !c.IsDeleted && c.Id == id)
                                                    .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            return category;
        }

        public async Task<long> InsertCategoryAsync(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentException("Name is required.");
            }

            try
            {
                var category = new Category
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = await _userService.GetUserId(),
                    UpdatedBy = await _userService.GetUserId(),
                    IsDeleted = false
                };

                await _context.Categories!.AddAsync(category);
                await _context.SaveChangesAsync();
                return category.Id;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the category.");
            }
        }
        public async Task<bool> UpdateCategoryAsync(long id, string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentException("Name is required.");
            }

            var category = await GetCategoryByIdAsync(id);

            try
            {
                category.Name = name;
                category.Description = description;
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = await _userService.GetUserId();
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while editing the category.");
            }
        }
        public async Task<bool> DeleteCategoryAsync(long id)
        {
            var category = await GetCategoryByIdAsync(id);

            try
            {
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = await _userService.GetUserId();
                category.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while deleting the category.");
            }
        }

    }
}