using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Scale
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

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
    }
}
