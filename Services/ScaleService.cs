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
        public ScaleService(ApplicationDbContext context)
        {
            _context = context ?? throw new InvalidOperationException("Context is not initialized.");
        }


        public async Task<List<Scale>> ListAllAsync()
        {
            CheckScalesInitialized();
            return await (_context.Scales!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync() ?? Task.FromResult(new List<Scale>()));
        }

        public async Task<List<Scale>> ListAllScaleAsync(int top)
        {
            CheckScalesInitialized();
            return await (_context.Scales!
                            .Where(c => !c.IsDeleted)
                            .OrderByDescending(c => c.CreatedAt)
                            .Take(top)
                            .ToListAsync() ?? Task.FromResult(new List<Scale>()));
        }

        public async Task<IPagedList<Scale>> ListAllAsync(string searchString, int page, int pageSize)
        {
            CheckScalesInitialized();

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

        public async Task<bool> CheckScaleAsync(string scaleName)
        {
            CheckScalesInitialized();

            return await _context.Scales!.AnyAsync(c => !c.IsDeleted
                                                        && c.Name != null
                                                        && c.Name.Equals(scaleName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<long> InsertAsync(Scale scale)
        {
            CheckScalesInitialized();

            if (scale == null)
            {
                throw new ArgumentNullException(nameof(scale), "Scale cannot be null.");
            }

            await _context.Scales!.AddAsync(scale);
            await _context.SaveChangesAsync();
            return scale.Id;
        }

        public async Task<Scale> GetScaleByIdAsync(long id)
        {
            CheckScalesInitialized();

            var scale = await _context.Scales!.FindAsync(id);

            if (scale == null || scale.IsDeleted)
            {
                throw new KeyNotFoundException($"Scale with ID {id} not found.");
            }

            return scale;
        }

        public async Task<Scale> ViewDetailAsync(long id)
        {
            CheckScalesInitialized();

            var scale = await _context.Scales!.FindAsync(id);

            if (scale == null || scale.IsDeleted)
            {
                throw new KeyNotFoundException($"Scale with ID {id} not found.");
            }

            return scale;
        }

        public async Task<bool> UpdateAsync(Scale scale)
        {
            try
            {
                CheckScalesInitialized();

                var existingScale = await _context.Scales!.FindAsync(scale.Id);

                if (existingScale == null || existingScale.IsDeleted)
                {
                    return false;
                }

                existingScale.Name = scale.Name;
                existingScale.Description = scale.Description;
                existingScale.UpdatedAt = scale.UpdatedAt;
                existingScale.UpdatedBy = scale.UpdatedBy;
                existingScale.IsDeleted = scale.IsDeleted;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckScalesInitialized()
        {
            if (_context?.Scales == null)
            {
                throw new InvalidOperationException("Scales DbSet is not initialized.");
            }
        }
    }
}