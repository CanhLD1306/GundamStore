using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

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

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
