using GundamStore.Admin.ViewModels;
using GundamStore.Models;

namespace GudandStore.Interfaces
{
    public interface IProductImageService
    {
        Task<List<ProductImage>> GetProductImagesByProductIdAsync(long id);
        Task<ProductImage> GetProductImageByIdAsync(long id);
        Task<ProductImage> GetImageDefaultAsync(long id);
        Task<long> InsertImagesAsync(long id, ImageViewModel model);
        Task<bool> UpdateImageAsync(ImageViewModel model);
        Task<bool> DeleteImagesAsync(long id);

    }
}