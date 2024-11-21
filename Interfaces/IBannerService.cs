using GundamStore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GundamStore.Interfaces
{
    public interface IBannerService
    {
        Task<List<Banner>> ListAllBannersAsync();
        Task<long> InsertBannerAsync(IFormFile fileImage, string description);
        Task<Banner> GetBannerByIdAsync(long id);
        Task<bool> UpdateBannerAsync(long id, string description);
        Task<bool> DeleteBannerAsync(long id);
    }
}
