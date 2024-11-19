using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class OrderItem
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long OrderId { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

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

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
