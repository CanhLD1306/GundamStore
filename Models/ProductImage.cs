using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class ProductImage
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        [Url]
        public string? ImageURL { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string? CreatedBy { get; set; }

        [Required]
        public string? UpdatedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public Product? Product { get; set; }
    }
}
