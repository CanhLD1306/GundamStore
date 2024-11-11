using GundamStore.Models;
using X.PagedList;

public interface ICategoryService
{
    Task<List<Category>> ListAllAsync();
    Task<List<Category>> ListAllCategoryAsync(int top);
    Task<IPagedList<Category>> ListAllAsync(string searchString, int page, int pageSize);
    Task<bool> CheckCategoryAsync(string categoryName);
    Task<long> InsertAsync(Category category);
    Task<Category> GetCategoryByIdAsync(long id);
    Task<Category> ViewDetailAsync(long id);
    Task<bool> UpdateAsync(Category category);
}