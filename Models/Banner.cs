using System;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Banner
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string? FileName { get; set; }

        [MaxLength(255)]
        public string? FileImage { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;

        [Required]
        public int Created_By { get; set; }

        [Required]
        public int Updated_By { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}