
using static System.Formats.Asn1.AsnWriter;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Product
    {

        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ScaleId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Brand { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int Discount { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public int  Sold { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

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

        public Category? Category { get; set; }
        public Scale? Scale { get; set; }

    }
}
