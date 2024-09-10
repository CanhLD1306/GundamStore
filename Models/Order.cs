using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(255)]
        public string? ShippingAddress { get; set; }

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
    }
}
