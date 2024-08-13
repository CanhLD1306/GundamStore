using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Banner
    {
        public int BannerId { get; set; }
        public string? FileName { get; set; }
        public string? FileImage { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

    }
}
