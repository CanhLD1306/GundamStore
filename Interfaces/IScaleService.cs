using GundamStore.Models;
using X.PagedList;

public interface IScaleService
{
    Task<List<Scale>> ListAllAsync();
    Task<List<Scale>> ListAllScaleAsync(int top);
    Task<IPagedList<Scale>> ListAllAsync(string searchString, int page, int pageSize);
    Task<bool> CheckScaleAsync(string scaleName);
    Task<long> InsertAsync(Scale scale);
    Task<Scale> GetScaleByIdAsync(long id);
    Task<Scale> ViewDetailAsync(long id);
    Task<bool> UpdateAsync(Scale scale);
}