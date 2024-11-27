using GundamStore.Models;
using X.PagedList;


namespace GundamStore.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<IPagedList<Category>> GetCategoriesAsync(string searchString, int page, int pageSize);
        Task<long> InsertCategoryAsync(string name, string description);
        Task<Category> GetCategoryByIdAsync(long id);
        Task<bool> UpdateCategoryAsync(long id, string name, string description);
        Task<bool> DeleteCategoryAsync(long id);
    }
}
