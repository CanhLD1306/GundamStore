using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Scale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

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
