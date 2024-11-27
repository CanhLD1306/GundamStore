using GundamStore.Models;
using X.PagedList;

namespace GundamStore.Interfaces
{
    public interface IScaleService
    {
        Task<List<Scale>> GetAllScalesAsync();
        Task<IPagedList<Scale>> GetScalesAsync(string searchString, int page, int pageSize);
        Task<long> InsertScaleAsync(string name, string description);
        Task<Scale> GetScaleByIdAsync(long id);
        Task<bool> UpdateScaleAsync(long id, string name, string description);
        Task<bool> DeleteScaleAsync(long id);
    }
}

