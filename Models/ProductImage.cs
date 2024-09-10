using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(1000)]
        public string? ImageURL { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        public DateTime Created_At { get; set; }

        [Required]
        public DateTime Updated_At { get; set; }

        [Required]
        public string Created_By { get; set; } = string.Empty;

        [Required]
        public string Updated_By { get; set; } = string.Empty;

        [Required]
        public bool IsDeleted { get; set; }

        public Product? Product { get; set; }
    }
}
