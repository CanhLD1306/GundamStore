using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Order
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public long StatusId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

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

        public Status? Status { get; set; }
    }
}
