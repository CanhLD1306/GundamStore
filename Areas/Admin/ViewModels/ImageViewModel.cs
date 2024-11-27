using System.ComponentModel.DataAnnotations;

namespace GundamStore.Admin.ViewModels
{
    public class ImageViewModel
    {
        public long Id { get; set; }
        public string? Url { get; set; }
        public IFormFile? File { get; set; }
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }

    }
}