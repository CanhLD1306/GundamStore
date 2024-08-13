using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Scale
    {
        [Key]
        public int ScaleId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

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
    }
}
