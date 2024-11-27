using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GundamStore.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; } = 0;

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public long ScaleId { get; set; }

        [MaxLength(255)]
        public string? Brand { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int Discount { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int StockQuantity { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}