using GundamStore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GundamStore.Interfaces
{
    public interface IBannerService
    {
        Task<long> CreateBannerAsync(IFormFile fileImage, string description);
        Task<List<Banner>> ListAllBannersAsync();
        Task<Banner> GetBannerByIdAsync(long id);
        Task<bool> UpdateBannerAsync(long id, IFormFile fileImage, string description);
        Task<bool> DeleteBannerAsync(long id);
    }
}
