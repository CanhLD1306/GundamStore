
using static System.Formats.Asn1.AsnWriter;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Product
    {

        [Key]
        public int Id { get; set; }

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
        public bool IsActive { get; set; }

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

        public Category? Category { get; set; }
        public Scale? Scale { get; set; }

    }
}
