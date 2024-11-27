using GundamStore.Admin.ViewModels;
using GundamStore.Areas.Admin.ViewModels;
using GundamStore.Dto;
using GundamStore.Models;
using X.PagedList;

namespace GundamStore.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductWithImageDto>> GetAllProductsAsync();
        Task<IPagedList<ProductWithImageDto>> GetProductsAsync(string searchString, int page, int pageSize);
        Task<Product> GetProductByIdAsync(long id);
        Task<long> InsertProductAsync(ProductViewModel model, List<ImageViewModel> images);
        Task<bool> UpdateProductAsync(ProductViewModel model, List<ImageViewModel> images);
        Task<bool> DeleteProductAsync(long id);
    }
}