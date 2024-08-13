using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string? ImageName { get; set; }

        [Required]
        [StringLength(1000)]
        public string? ImageURL { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public Product? Product { get; set; }
    }
}
