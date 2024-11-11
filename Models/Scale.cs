using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Scale
    {
        [Key]
        public long Id { get; set; }

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
        public string? CreatedBy { get; set; }

        [Required]
        public string? UpdatedBy { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
