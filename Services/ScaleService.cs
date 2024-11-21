using GundamStore.Data;
using GundamStore.Models;
using GundamStore.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;


namespace GundamStore.Services
{
    public class ScaleService : IScaleService
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserService _userService;
        public ScaleService(ApplicationDbContext context, IUserService userService)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
            _userService = userService ?? throw new InvalidOperationException("UserService is not initialized.");
        }


        public async Task<List<Scale>> ListAllScalesAsync()
        {
            return await _context.Scales!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync();
        }

        public async Task<IPagedList<Scale>> ListAllScalesAsync(string searchString, int page, int pageSize)
        {


            IQueryable<Scale> scale = _context.Scales!.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                scale = scale.Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchString));
            }

            var result = await scale.OrderByDescending(c => c.CreatedAt)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            return new PagedList<Scale>(result, page, pageSize);
        }

        public async Task<Scale> GetScaleByIdAsync(long id)
        {

            var scale = await _context.Scales!.Where(s => !s.IsDeleted && s.Id == id)
                                                    .FirstOrDefaultAsync();

            if (scale == null)
            {
                throw new KeyNotFoundException("Scale not found.");
            }

            return scale;
        }

        public async Task<long> InsertScaleAsync(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentException("Name is required.");
            }

            try
            {
                var scale = new Scale
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = await _userService.GetUserId(),
                    UpdatedBy = await _userService.GetUserId(),
                    IsDeleted = false
                };

                await _context.Scales!.AddAsync(scale);
                await _context.SaveChangesAsync();
                return scale.Id;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the scale.");
            }
        }

        public async Task<bool> UpdateScaleAsync(long id, string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentException("Name is required.");
            }

            var scale = await GetScaleByIdAsync(id);

            try
            {
                scale.Name = name;
                scale.Description = description;
                scale.UpdatedAt = DateTime.Now;
                scale.UpdatedBy = await _userService.GetUserId();
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while editing the scale.");
            }
        }

        public async Task<bool> DeleteScaleAsync(long id)
        {
            var scale = await GetScaleByIdAsync(id);

            try
            {
                scale.UpdatedAt = DateTime.Now;
                scale.UpdatedBy = await _userService.GetUserId();
                scale.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while deleting the scale.");
            }
        }
    }
}