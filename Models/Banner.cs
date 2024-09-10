using System;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Banner
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string? FileImage { get; set; }

        [MaxLength(500)]
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